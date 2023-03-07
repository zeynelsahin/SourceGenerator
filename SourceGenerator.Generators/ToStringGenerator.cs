using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Generators.Model;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SourceGenerator.Generators;

[Generator]
public class ToStringGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => IsSyntaxNode(node),
            transform: static (ctx, _) => GetSemanticTarget(ctx)).Where(static target => target is not null).Collect();

        context.RegisterSourceOutput(classes, static (ctx, source) => Execute(ctx, source));

        context.RegisterPostInitializationOutput(static (ctx) => PostInitializationOutput(ctx));
    }

    private static bool IsSyntaxNode(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0;
    }
    private static ClassToGenerate? GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
        var attributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("SourceGenerator.Generators.GenerateToStringAttribute");
        if (classSymbol is not null && attributeSymbol is not null)
        {
            foreach (var attributeData in classSymbol.GetAttributes())
            {
                if (attributeSymbol.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                {
                    var nameSpaceName = classSymbol.ContainingNamespace.ToDisplayString();
                    var className = classSymbol.Name;
                    var properrtNames = new List<string>();

                    foreach (var memeberSymbol in classSymbol.GetMembers())
                    {
                        if (memeberSymbol.Kind == SymbolKind.Property && memeberSymbol.DeclaredAccessibility == Accessibility.Public)
                        {
                            properrtNames.Add(memeberSymbol.Name);
                        }
                    }
                    return new ClassToGenerate(nameSpaceName, className, properrtNames);
                }
            }
        }
        return null;
    }

    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("SourceGenerator.Generators.GenerateToStringAttributte.g.cs", @"namespace SourceGenerator.Generators
{
    internal class GenerateToStringAttribute : System.Attribute { }   
}");
    }

    private static Dictionary<string, int> _countPerFileName = new Dictionary<string, int>();
    private static void Execute(SourceProductionContext context, ImmutableArray<ClassToGenerate?> classesToGenerate)
    {
        foreach (var classToGenerate in classesToGenerate)
        {
            if (classToGenerate is null)
            {
                return;
            }

            var nameSpace = classToGenerate.NameSpaceName;

            var className = classToGenerate.ClassName;
            var fileName = $"{nameSpace}.{className}.g.cs";
            if (_countPerFileName.ContainsKey(fileName))
            {
                _countPerFileName[fileName]++;
            }
            else
            {
                _countPerFileName.Add(fileName, 1);
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"//Generation count : {_countPerFileName[fileName]}
namespace {nameSpace}
{{
    partial class {className}
    {{
        public override string ToString()
        {{
            return $""");

            var first = true;
            foreach (var propertyName in classToGenerate.PropertyNames)
            {

                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.Append("; ");
                }

                stringBuilder.Append($"{propertyName}:{{{propertyName}}}");

            }
            stringBuilder.Append($@""";
        }}
    }}
}}
");
            context.AddSource(fileName, stringBuilder.ToString());
        }
    }
}
