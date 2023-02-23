using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace SourceGenerator.Generators
{
    [Generator]
    public class ToStringGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classes = context.SyntaxProvider.CreateSyntaxProvider(predicate: (node, _) => node is ClassDeclarationSyntax, transform: (ctx, _) => (ClassDeclarationSyntax)ctx.Node);

            context.RegisterSourceOutput(classes, (ctx, source) => Execute(ctx, source));
        }

        private void Execute(SourceProductionContext context, ClassDeclarationSyntax classDeclarationSyntax)
        {

        }
    }
}
