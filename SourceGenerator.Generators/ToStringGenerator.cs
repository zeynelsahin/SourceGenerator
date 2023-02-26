﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    }

    private static void Execute(SourceProductionContext context, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (classDeclarationSyntax.Parent is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
        {

            var nameSpace = namespaceDeclarationSyntax.Name.ToString();

            var className = classDeclarationSyntax.Identifier.Text;
            var fileName = $"{nameSpace}.{className}.g.cs";
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"namespace {nameSpace}
{{
    partial class {className}
    {{
        
    }}
}}
");
            context.AddSource(fileName, stringBuilder.ToString());
        }
    }
}
