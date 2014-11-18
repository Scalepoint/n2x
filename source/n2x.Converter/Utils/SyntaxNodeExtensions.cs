using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Utils
{
    public static class SyntaxNodeExtensions
    {
        public static ClassDeclarationSyntax FirstClass(this SyntaxNode root)
        {
            return root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
        }
    }
}