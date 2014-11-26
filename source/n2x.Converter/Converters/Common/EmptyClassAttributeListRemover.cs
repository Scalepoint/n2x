using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Common
{
    public class EmptyClassAttributeListRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var emptyLists = root.Classes()
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