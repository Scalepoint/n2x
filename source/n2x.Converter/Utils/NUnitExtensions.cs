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
            return @class.GetClassMethods<TestFixtureSetUpAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestFixtureTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<TestFixtureTearDownAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTearDownMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<TearDownAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<TestAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetTestCaseMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<TestCaseAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetExplicitMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<ExplicitAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<SetUpAttribute>(semanticModel);
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

        public static bool HasSetUpMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetSetUpMethods(semanticModel).Any();
        }

        public static bool IsNunitAssert(this ISymbol symbol)
        {
            return symbol.IsOfType<Assert>();
        }

        public static bool IsNunitAssert(this InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(invocation).Symbol;
            return symbol != null && symbol.IsNunitAssert();
        }

        public static bool IsAssertionException(this TypeSyntax @type, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetSymbolInfo(@type).Symbol;
            return symbol.IsOfType<AssertionException>();
        }
    }
}