using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureAttribute
{
    public class TestFixtureAttributeRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var attributesToRemove = root.Classes().DecoratedWith<NUnit.Framework.TestFixtureAttribute>(semanticModel)
                .SelectMany(c => c.GetAttributes<NUnit.Framework.TestFixtureAttribute>(semanticModel))
                .ToList();

            if (attributesToRemove.Any())
            {
                return root.RemoveNodes(attributesToRemove, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
    }
}