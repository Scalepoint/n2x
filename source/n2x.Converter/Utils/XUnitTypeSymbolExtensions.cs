using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace n2x.Converter.Utils
{
    public static class XunitTypeSymbolExtensions
    {
        //TODO: use IsOfType<>()
        public static bool IsIClassFixtureInterface(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString().StartsWith(@"Xunit.IClassFixture<");
        }

        //TODO: use IsOfType<>()
        public static bool IsIClassFixtureInterfaceOf(this ITypeSymbol typeSymbol, string arg)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == string.Format("Xunit.IClassFixture<{0}>", arg);
        }

        public static IEnumerable<ClassDeclarationSyntax> GetIClassFixtureClasses(this SyntaxNode root, SemanticModel semanticModel)
        {
            return root.Classes()
                .Where(c => c.BaseList?.Types.Any(t => semanticModel.GetTypeInfo(t).Type.IsIClassFixtureInterface()) ?? false);
        }

        public static string GetIClassFixtureTypeArgumentName(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.BaseList.Types.Select(t => (INamedTypeSymbol) semanticModel.GetTypeInfo(t).Type)
                .Single(m => m.IsIClassFixtureInterface())
                .TypeArguments
                .Single().Name;
        }

        public static bool IsTraitAttributeWith(this AttributeSyntax attribute, string key, string value,
            SemanticModel semanticModel)
        {
            if (attribute.ArgumentList?.Arguments.Count != 2)
            {
                return false;
            }

            var keyArgInfo = semanticModel.GetConstantValue(attribute.ArgumentList.Arguments.First().Expression);
            var valueArg = semanticModel.GetConstantValue(attribute.ArgumentList.Arguments.Last().Expression);

            return attribute.IsOfType<TraitAttribute>(semanticModel)
                && keyArgInfo.HasValue && (string)keyArgInfo.Value == key
                && valueArg.HasValue && (string)valueArg.Value == value;
        }

        public static IEnumerable<MethodDeclarationSyntax> GetFactMethods(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<FactAttribute>(semanticModel);
        }
    }
}