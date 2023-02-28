using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node);

        context.RegisterSourceOutput(classes, static (ctx, source) => Execute(ctx, source));

        context.RegisterPostInitializationOutput(static (ctx) => PostInitializationOutput(ctx));
    }
    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("SourceGenerator.Generators.GenerateToStringAttributte.g.cs", @"namespace SourceGenerator.Generators
{
    internal class GenerateToStringAttribute : System.Attribute { }   
}");
    }
    private static void Execute(SourceProductionContext context, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (classDeclarationSyntax.Parent is BaseNamespaceDeclarationSyntax namespaceDeclarationSyntax)
        {

            var nameSpace = namespaceDeclarationSyntax.Name.ToString();

            var className = classDeclarationSyntax.Identifier.Text;
            var fileName = $"{nameSpace}.{className}.g.cs";
            var stringBuilder = new StringBuilder();
            if (className.Equals("GenerateToStringAttribute"))
            {
                return;
            }
            stringBuilder.Append($@"namespace {nameSpace}
{{
    partial class {className}
    {{
        public override string ToString()
        {{
            return $""");

            var first = true;
            foreach (var memberDeclarationSyntax in classDeclarationSyntax.Members)
            {
                if (memberDeclarationSyntax is PropertyDeclarationSyntax propertyDeclarationSyntax && propertyDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword)) 
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        stringBuilder.Append("; ");
                    }
                    var propertyName = propertyDeclarationSyntax.Identifier.Text;

                    stringBuilder.Append($"{propertyName}:{{{propertyName}}}");

                }
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
