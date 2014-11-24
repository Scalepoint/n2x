using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace n2x.Converter.Utils
{
    public static class NUnitExtensions
    {
        public static IEnumerable<MethodDeclarationSyntax> GetTestFixtureSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => a.IsOfType<TestFixtureSetUpAttribute>(semanticModel)));
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestFixtureTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => a.IsOfType<TestFixtureTearDownAttribute>(semanticModel)));
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => a.IsOfType<TearDownAttribute>(semanticModel)));
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => a.IsOfType<TestAttribute>(semanticModel)));
        }

        public static bool HasTestFixtureSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetTestFixtureSetUpMethods(semanticModel).Any();
        }

        public static bool HasTestFixtureTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetTestFixtureTearDownMethods(semanticModel).Any();
        }

        public static bool HasTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetTearDownMethods(semanticModel).Any();
        }
    }
}