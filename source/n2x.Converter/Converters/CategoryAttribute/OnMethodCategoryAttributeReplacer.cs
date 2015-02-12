using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.CategoryAttribute
{
    public class OnMethodCategoryAttributeReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var methods = root.Classes().SelectMany(p => p.Methods().DecoratedWith<NUnit.Framework.CategoryAttribute>(semanticModel));
            foreach (var method in methods)
            {
                var categoryAttributes = method.GetAttributes<NUnit.Framework.CategoryAttribute>(semanticModel);
                foreach (var categoryAttribute in categoryAttributes)
                {
                    var categoryArg = categoryAttribute.ArgumentList.Arguments.Single();
                    var key = SyntaxFactory.AttributeArgument(ExpressionGenerator.GenerateValueExpression("Category"));
                    var value = SyntaxFactory.AttributeArgument(categoryArg.Expression);

                    var traitAttrDeclaration = ExpressionGenerator.GenerateAttribute<TraitAttribute>(key, value);

                    dict.Add(categoryAttribute, traitAttrDeclaration);
                }
            }

            return root.ReplaceNodes(dict);

        }
    }
}