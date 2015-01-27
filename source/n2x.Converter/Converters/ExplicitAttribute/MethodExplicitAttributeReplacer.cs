using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.ExplicitAttribute
{
    public class MethodExplicitAttributeReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            var methods = root.Classes().SelectMany(p => p.GetExplicitMethods(semanticModel));

            foreach (var method in methods)
            {
                var factAttribute = method.GetAttribute<FactAttribute>(semanticModel);

                var newFactAttribute =
                        ExpressionGenerator.GenerateAttribute<FactAttribute>(
                            SyntaxFactory.AttributeArgument(SyntaxFactory.ParseExpression("Skip=\"Explicit\"")));

                if (factAttribute == null)
                {
                    var newMethod = method.AddAtribute(newFactAttribute);

                    dict.Add(method, newMethod);
                }
                else
                {
                    dict.Add(factAttribute, newFactAttribute);
                }
            }

            return root.ReplaceNodes(dict);
        }
    }
}