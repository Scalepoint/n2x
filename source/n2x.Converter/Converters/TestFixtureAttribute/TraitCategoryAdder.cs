using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;

namespace n2x.Converter.Converters.TestFixtureAttribute
{
    public class TraitCategoryAdder : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var testFixture in root.Classes().DecoratedWith<NUnit.Framework.TestFixtureAttribute>(semanticModel))
            {
                var testFixtureAttribute = testFixture.GetAttribute<NUnit.Framework.TestFixtureAttribute>(semanticModel);

                var categoryArg = testFixtureAttribute.ArgumentList?.Arguments.SingleOrDefault(a => a.NameEquals.Name.Identifier.Text == "Category");
                if (categoryArg != null)
                {
                    var key = SyntaxFactory.AttributeArgument(ExpressionGenerator.GenerateValueExpression("Category"));
                    var value = SyntaxFactory.AttributeArgument(categoryArg.Expression);

                    var traitAttrDeclaration = ExpressionGenerator.GenerateAttribute<TraitAttribute>(key, value);
                    dict.Add(testFixtureAttribute, traitAttrDeclaration);
                }
            }

            //TODO: we should insert new attribute instead here and imiplement TestFixtureAttributeRemover instead
            //but I didn't find the way to insert several nodes in several different locations with a single call
            return root.ReplaceNodes(dict);
        }
    }
}