using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.ExplicitAttribute
{
    public class ExplicitAttributeReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var methods = root.Classes().SelectMany(p => p.GetExplicitMethods(semanticModel));
            var attributes = methods.SelectMany(p => p.GetAttributes<NUnit.Framework.ExplicitAttribute>(semanticModel));

            var argument = SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression("Skip=\"Explicit\""));

            if (attributes.Any())
            {
                return root.ReplaceNodes(attributes, (n1, n2) => ExpressionGenerator.GenerateAttribute<FactAttribute>(argument)).NormalizeWhitespace();
            }
            return root;
        }
    }
}