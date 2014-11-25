using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.SetUp
{
    public class SetUpMethodRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var setUpMethods = root
                .Classes()
                .SelectMany(c => c.GetSetUpMethods(semanticModel))
                .ToList();

            if (setUpMethods.Any())
            {
                return root.RemoveNodes(setUpMethods, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;

        }
    }
}