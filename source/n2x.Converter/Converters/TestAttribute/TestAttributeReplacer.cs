using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;
using Xunit;
using n2x.Converter.Generators;

namespace n2x.Converter.Converters.TestAttribute
{
    public class TestAttributeReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var methods = root.Classes().SelectMany(p => p.GetTestMethods(semanticModel));
            var attributes = methods.SelectMany(p => p.GetAttributes<NUnit.Framework.TestAttribute>(semanticModel));

            return root.ReplaceNodes(attributes, (n1, n2) => ExpressionGenerator.GenerateAttribute<FactAttribute>()).NormalizeWhitespace();
        }
    }
}
