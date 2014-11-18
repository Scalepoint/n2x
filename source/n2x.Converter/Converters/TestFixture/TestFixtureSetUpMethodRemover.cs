using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureSetUpMethodRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var testFixtureSetUpMethods = root
                .Classes()
                .SelectMany(c => c.GetTestFixtureSetUpMethods(semanticModel))
                .ToList();

            if (testFixtureSetUpMethods.Any())
            {
                return root.RemoveNodes(testFixtureSetUpMethods, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
    }
}