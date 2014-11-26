using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestCaseAttribute
{
    public class TestCaseAttributeRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var methods = root
                .Classes()
                .SelectMany(c => c.GetTestCaseMethods(semanticModel))
                .ToList();
            var attributes = methods.SelectMany(p => p.GetAttributes<NUnit.Framework.TestCaseAttribute>(semanticModel)).ToList();

            if (attributes.Any())
            {
                return root.RemoveNodes(attributes, SyntaxRemoveOptions.KeepNoTrivia).NormalizeWhitespace();
            }

            return root;
        }
    }
}