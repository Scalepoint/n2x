﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Utils
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<ClassDeclarationSyntax> Classes(this SyntaxNode root)
        {
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        }

        public static ClassDeclarationSyntax FirstClass(this SyntaxNode root)
        {
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        }

        public static bool IsOfType<T>(this SyntaxNode @this, SemanticModel semanticModel)
        {
            var typeSymbol = semanticModel.GetTypeInfo(@this).Type;
            return !typeSymbol.IsErrorType() &&
                   typeof (T).FullName == typeSymbol.ToDisplayString();
        }

        public static IEnumerable<ClassDeclarationSyntax> DecoratedWith<T>(this IEnumerable<ClassDeclarationSyntax> @this, SemanticModel semanticModel)
            where T : Attribute
        {
            return @this.Where(c => c.AttributeLists
                .SelectMany(l => l.Attributes)
                .Any(a => a.IsOfType<T>(semanticModel)));
        }

        public static IEnumerable<MethodDeclarationSyntax> DecoratedWith<T>(this IEnumerable<MethodDeclarationSyntax> @this, SemanticModel semanticModel)
            where T : Attribute
        {
            return @this.Where(c => c.AttributeLists
                .SelectMany(l => l.Attributes)
                .Any(a => a.IsOfType<T>(semanticModel)));
        }

        public static IEnumerable<AttributeSyntax> GetAttributes<T>(this ClassDeclarationSyntax @this, SemanticModel semanticModel)
            where T : Attribute
        {
            return @this.AttributeLists
                .SelectMany(l => l.Attributes)
                .Where(a => a.IsOfType<T>(semanticModel));
        }

        public static AttributeSyntax GetAttribute<T>(this ClassDeclarationSyntax @this, SemanticModel semanticModel)
            where T : Attribute
        {
            return @this.GetAttributes<T>(semanticModel).SingleOrDefault();
        }

        public static IEnumerable<AttributeSyntax> GetAttributes<T>(this MethodDeclarationSyntax @this, SemanticModel semanticModel)
            where T : Attribute
        {
            return @this.AttributeLists
                .SelectMany(l => l.Attributes)
                .Where(a => a.IsOfType<T>(semanticModel));
        }

        public static bool HasDisposableBaseClass(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            var baseClasses = @class.BaseList.Types.OfType<IdentifierNameSyntax>()
                .Where(p => !p.Identifier.Text.StartsWith("I"));

            foreach (var baseClass in baseClasses)
            {
                var baseClassType = semanticModel.GetTypeInfo(baseClass).Type;
                var hasInterface = baseClassType.Interfaces.Any(p => p.Name == "IDisposable");

                return baseClassType.TypeKind == TypeKind.Class && !baseClassType.IsAbstract && hasInterface;
            }

            return false;
        }

        public static MethodDeclarationSyntax GetDisposeMethod(this ClassDeclarationSyntax @class)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>().FirstOrDefault(m => m.Identifier.Text == "Dispose");
        }

        public static bool HasDisposeMethod(this ClassDeclarationSyntax @class)
        {
            return @class.GetDisposeMethod() != null;
        }

        public static bool IsDisposable(this ClassDeclarationSyntax @class)
        {
            return @class.BaseList != null
                && @class.BaseList.Types.OfType<IdentifierNameSyntax>().Any(p => p.Identifier.Text == "IDisposable");
        }

        public static IEnumerable<ClassDeclarationSyntax> WithSetUpMethods(this IEnumerable<ClassDeclarationSyntax> @this, SemanticModel semanticModel)
        {
            return @this.Where(c => c.GetSetUpMethods(semanticModel).Any());
        }
    }
}