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
            var stronglyTypedID = PrepareStronglyTypedID(symbolName, symbolNamespace.ToString());
            context.AddSource($"SimpleAPI.{symbolName}.TypedID.g.cs", stronglyTypedID);

            // var typeIDConversion = PrepareTypeIDConversion(symbolName, symbolNamespace.ToString());
            // context.AddSource($"SimpleAPI.{symbolName}.EntityConversion.g.cs", typeIDConversion);
        }
    }

    private static string PrepareStronglyTypedID(string symbolName, string symbolNamespace)
    {
        // https://andrewlock.net/strongly-typed-id-updates/
        return $$"""
                 using SimpleAPI.Domain.Base;

                 namespace {{symbolNamespace}};

                 public readonly struct {{symbolName}}ID : IComparable<{{symbolName}}ID>, IEquatable<{{symbolName}}ID>, IEntityID
                 {
                     public int Value { get; }
                 
                     public {{symbolName}}ID(int value) => Value = value;
                     
                     
                     public bool Equals({{symbolName}}ID other) => this.Value.Equals(other.Value);
                     public int CompareTo({{symbolName}}ID other) => Value.CompareTo(other.Value);
                 
                     public override bool Equals(object obj)
                     {
                         if (ReferenceEquals(null, obj)) return false;
                         return obj is {{symbolName}}ID other && Equals(other);
                     }
                 
                     public override int GetHashCode() => Value.GetHashCode();
                     public override string ToString() => Value.ToString();
                 
                     public static bool operator ==({{symbolName}}ID a, {{symbolName}}ID b) => a.CompareTo(b) == 0;
                     public static bool operator !=({{symbolName}}ID a, {{symbolName}}ID b) => !(a == b);
                 }
                 """;
    }

    private static string PrepareTypeIDConversion(string symbolName, string symbolNamespace)
    {
        return $$"""
                 using {{symbolNamespace}};
                 using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

                 namespace SimpleAPI.Infrastructure.Persistence.Configuration;

                 public class {{symbolName}}IDConverter : ValueConverter<{{symbolName}}ID, int>
                 {
                     public {{symbolName}}IDConverter() : base(
                        v => v.Value,
                        v => new {{symbolName}}ID(v)
                    ) { }
                 }
                 """;
    }
}
