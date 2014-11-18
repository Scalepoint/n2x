using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Utils
{
    public static class NUnitExtensions
    {
        public static bool IsTestFixtureSetUpAttribute(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == "NUnit.Framework.TestFixtureSetUpAttribute";
        }

        public static bool HasTestSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Any(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => semanticModel.GetTypeInfo(a).Type.IsTestFixtureSetUpAttribute()));
        }
    }
}