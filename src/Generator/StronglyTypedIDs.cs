using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SimpleAPI.Generator;

// Ideas on https://www.meziantou.net/strongly-typed-ids-with-csharp-source-generators.htm

[Generator(LanguageNames.CSharp)]
public class StronglyTypedIDs : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "SimpleAPI.Generator.Common.g.cs",
            SourceText.From(Helpers.StronglyTypeAttributeNameCode, Encoding.UTF8)));

        var entityTypes = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => IsNodeTargetForGeneration(node),
            transform: (syntaxContext, token) => (ClassDeclarationSyntax)syntaxContext.Node
        );

        var compilation = context.CompilationProvider.Combine(entityTypes.Collect());
        context.RegisterSourceOutput(compilation, (productionContext, syntax)
            => Execute(productionContext, syntax.Left, syntax.Right));
    }

    private static bool IsNodeTargetForGeneration(SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al
                    .Attributes
                    .Any(a => a.Name.ToString() == Helpers.StronglyTypeAttributeName ||
                        a.Name.ToString() == (Helpers.StronglyTypeAttributeName + "Attribute")));

    private static void Execute(SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> typesList)
    {
        foreach (var syntax in typesList)
        {
            var model = compilation.GetSemanticModel(syntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(syntax);
            if (symbol is null)
                continue;

            var symbolName = symbol.Name;
            var symbolNamespace = symbol.ContainingNamespace;
            var code = $$"""
                         using SimpleAPI.Domain.Base;

                         namespace {{symbolNamespace}};

                         public class {{symbolName}}ID : EntityID
                         {
                             public {{symbolName}}ID(int id) : base(id) { }
                             public {{symbolName}}ID() : base(0) { }
                         }
                         """;
            var hintName = $"SimpleAPI.{symbolName}.StronglyTypedID.g.cs";
            context.AddSource(hintName, code);
        }
    }
}
