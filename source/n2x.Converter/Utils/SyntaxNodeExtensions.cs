using System;
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
    }
}