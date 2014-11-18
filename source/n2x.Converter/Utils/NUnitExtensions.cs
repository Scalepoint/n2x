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

        public static bool IsTestFixtureTearDownAttribute(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == "NUnit.Framework.TestFixtureTearDownAttribute";
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestFixtureSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => semanticModel.GetTypeInfo(a).Type.IsTestFixtureSetUpAttribute()));
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestFixtureTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => semanticModel.GetTypeInfo(a).Type.IsTestFixtureTearDownAttribute()));
        }

        public static bool HasTestFixtureSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetTestFixtureSetUpMethods(semanticModel).Any();
        }

        public static bool HasTestFixtureTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetTestFixtureTearDownMethods(semanticModel).Any();
        }
    }
}