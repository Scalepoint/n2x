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
    }
}