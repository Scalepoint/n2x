using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace n2x.Converter.Utils
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<ClassDeclarationSyntax> Classes(this SyntaxNode root)
        {
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        }

        public static IEnumerable<MethodDeclarationSyntax> Methods(this ClassDeclarationSyntax @class)
        {
            return @class.Members.OfType<MethodDeclarationSyntax>();
        }

        public static IEnumerable<ConstructorDeclarationSyntax> Ctors(this ClassDeclarationSyntax @class)
        {
            return @class.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
        }

        public static IEnumerable<UsingDirectiveSyntax> Usings(this SyntaxNode root)
        {
            return root.DescendantNodes().OfType<UsingDirectiveSyntax>();
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

        public static AttributeSyntax GetAttribute<T>(this MethodDeclarationSyntax @this, SemanticModel semanticModel)
            where T : Attribute
        {
            return @this.GetAttributes<T>(semanticModel).SingleOrDefault();
        }

        public static bool HasDisposableBaseClass(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            foreach (var baseClass in @class.BaseList.Types)
            {
                var baseClassType = semanticModel.GetTypeInfo(baseClass.Type).Type;
                var hasInterface = baseClassType.Interfaces.Any(p => p.Name == "IDisposable");

                return baseClassType.TypeKind == TypeKind.Class && !baseClassType.IsAbstract && hasInterface;
            }

            return false;
        }

        public static MethodDeclarationSyntax GetDisposeMethod(this ClassDeclarationSyntax @class)
        {
            return @class.Methods().FirstOrDefault(m => m.Identifier.Text == "Dispose");
        }

        public static bool HasDisposeMethod(this ClassDeclarationSyntax @class)
        {
            return @class.GetDisposeMethod() != null;
        }

        public static bool IsDisposable(this ClassDeclarationSyntax @class)
        {
            return @class.BaseList != null
                   && @class.BaseList.Types.Any(p =>
                       (p.Type.IsKind(SyntaxKind.QualifiedName) && ((QualifiedNameSyntax) p.Type).Right.Identifier.Text == "IDisposable")
                       ||
                       (p.Type.IsKind(SyntaxKind.IdentifierName) && ((IdentifierNameSyntax) p.Type).Identifier.Text == "IDisposable")
                       );
        }

        public static IEnumerable<ClassDeclarationSyntax> WithSetUpMethods(this IEnumerable<ClassDeclarationSyntax> @this, SemanticModel semanticModel)
        {
            return @this.Where(c => c.HasSetUpMethods(semanticModel));
        }

        public static MethodDeclarationSyntax AddAtribute(this MethodDeclarationSyntax method, AttributeSyntax attribute)
        {
            var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[] { attribute }));
            return method.AddAttributeLists(attributeList);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetClassMethods<T>(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.Methods()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => a.IsOfType<T>(semanticModel)));
        }

        public static SyntaxNode ReplaceNodes(this SyntaxNode root, Dictionary<SyntaxNode, SyntaxNode> dict, bool normalize = true)
        {
            if (dict.Any())
            {
                var newRoot = root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]);
                return normalize ? newRoot.NormalizeWhitespace() : newRoot;
            }

            return root;
        }

        public static SyntaxNode RemoveNodes(this SyntaxNode root, IEnumerable<SyntaxNode> attributes)
        {
            if (attributes.Any())
            {
                return root.RemoveNodes(attributes, SyntaxRemoveOptions.KeepNoTrivia).NormalizeWhitespace();
            }

            return root;
        }

        public static bool IsPublic(this ClassDeclarationSyntax @class)
        {
            return @class.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));
        }

        public static string GetAssemblyName(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            var symbolInfo = semanticModel.GetDeclaredSymbol(@class);
            return symbolInfo.ContainingAssembly?.Identity.ToString();
        }

        public static string GetAssemblyName(this TypeSyntax node, SemanticModel semanticModel)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(node);
            var symbol = symbolInfo.Symbol;
            if (symbol == null && symbolInfo.CandidateSymbols.Any())
            {
                symbol = symbolInfo.CandidateSymbols.First();
            }

            Debug.Assert(symbol != null);
            return symbol.ContainingAssembly.Identity.ToString();
        }
    }
}