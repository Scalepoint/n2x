using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.CategoryAttribute
{
    public class OnClassCategoryAttributeReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var classes = root.Classes().DecoratedWith<NUnit.Framework.CategoryAttribute>(semanticModel);
            foreach (var @class in classes)
            {
                var categoryAttributes = @class.GetAttributes<NUnit.Framework.CategoryAttribute>(semanticModel);
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