using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Utils
{
    public static class XUnitTypeSymbolExtensions
    {
        public static bool IsIUseFixtureInterface(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString().StartsWith(@"Xunit.IUseFixture<");
        }

        public static bool IsIUseFixtureInterfaceOf(this ITypeSymbol typeSymbol, string arg)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == string.Format("Xunit.IUseFixture<{0}>", arg);
        }

        public static IEnumerable<ClassDeclarationSyntax> GetIUseFixtureClasses(this SyntaxNode root, SemanticModel semanticModel)
        {
            return root.Classes()
                .Where(c => c.BaseList != null
                            && c.BaseList.Types
                                .Any(t => semanticModel.GetTypeInfo(t)
                                    .Type.IsIUseFixtureInterface()));
        }

        public static string GetIUseFixtureTypeArgumentName(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.BaseList.Types.Select(t => (INamedTypeSymbol) semanticModel.GetTypeInfo(t).Type)
                .Single(m => m.IsIUseFixtureInterface())
                .TypeArguments
                .Single().Name;
        }
    }
}