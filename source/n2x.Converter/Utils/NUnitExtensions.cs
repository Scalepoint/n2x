using System.Collections.Generic;
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

        public static IEnumerable<MethodDeclarationSyntax> GetTestSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => semanticModel.GetTypeInfo(a).Type.IsTestFixtureSetUpAttribute()));
        }

        public static bool HasTestSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetTestSetUpMethods(semanticModel).Any();
        }
    }
}