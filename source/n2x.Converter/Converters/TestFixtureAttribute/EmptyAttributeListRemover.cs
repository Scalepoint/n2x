using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureAttribute
{
    public class EmptyAttributeListRemover : IConverter
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