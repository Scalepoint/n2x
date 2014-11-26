using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Common
{
    public class EmptyMethodAttributeListRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var emptyLists = root.Classes()
                .SelectMany(p => p.Members.OfType<MethodDeclarationSyntax>())
                .SelectMany(c => c.AttributeLists)
                .Where(l => !l.Attributes.Any())
                .ToList();

            if (emptyLists.Any())
            {
                return root.RemoveNodes(emptyLists, SyntaxRemoveOptions.KeepNoTrivia).NormalizeWhitespace();
            }

            return root;
        }
    }
}