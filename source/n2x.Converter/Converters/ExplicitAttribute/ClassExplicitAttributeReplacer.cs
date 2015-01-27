using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.ExplicitAttribute
{
    public class ClassExplicitAttributeReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            var classes = root.Classes().DecoratedWith<NUnit.Framework.ExplicitAttribute>(semanticModel);

            foreach (var @class in classes)
            {
                var factMethods = @class.GetFactMethods(semanticModel).ToList();

                foreach (var method in factMethods)
                {
                    var factAttribute = method.GetAttribute<FactAttribute>(semanticModel);

                    var newFactAttribute =
                        ExpressionGenerator.GenerateAttribute<FactAttribute>(
                            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression("Skip=\"Explicit\"")));

                    dict.Add(factAttribute, newFactAttribute);
                }
            }

            return root.ReplaceNodes(dict);
        }
    }
}