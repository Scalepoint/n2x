﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace n2x.Converter.Utils
{
    public static class XunitTypeSymbolExtensions
    {
        //TODO: use IsOfType<>()
        public static bool IsIUseFixtureInterface(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString().StartsWith(@"Xunit.IUseFixture<");
        }

        //TODO: use IsOfType<>()
        public static bool IsIUseFixtureInterfaceOf(this ITypeSymbol typeSymbol, string arg)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == string.Format("Xunit.IUseFixture<{0}>", arg);
        }

        public static IEnumerable<ClassDeclarationSyntax> GetIUseFixtureClasses(this SyntaxNode root, SemanticModel semanticModel)
        {
            return root.Classes()
                .Where(c => c.BaseList?.Types.Any(t => semanticModel.GetTypeInfo(t).Type.IsIUseFixtureInterface()) ?? false);
        }

        public static string GetIUseFixtureTypeArgumentName(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.BaseList.Types.Select(t => (INamedTypeSymbol) semanticModel.GetTypeInfo(t).Type)
                .Single(m => m.IsIUseFixtureInterface())
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
    }
}