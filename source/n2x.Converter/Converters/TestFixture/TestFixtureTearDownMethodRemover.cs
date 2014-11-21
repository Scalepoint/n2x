using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureTearDownMethodRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var testFixtureTearDownMethods = root
                .Classes()
                .SelectMany(c => c.GetTestFixtureTearDownMethods(semanticModel))
                .ToList();

            if (testFixtureTearDownMethods.Any())
            {
                return root.RemoveNodes(testFixtureTearDownMethods, SyntaxRemoveOptions.KeepNoTrivia).NormalizeWhitespace();
            }

            return root;
        }
    }
}