using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;
using System.Linq;

namespace n2x.Converter.Converters.TearDownAttribute
{
    internal class TearDownMethodRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var tearDownMethods = root
                .Classes()
                .SelectMany(c => c.GetTearDownMethods(semanticModel))
                .ToList();

            if (tearDownMethods.Any())
            {
                return root.RemoveNodes(tearDownMethods, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
    }
}