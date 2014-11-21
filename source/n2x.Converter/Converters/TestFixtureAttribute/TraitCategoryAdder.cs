using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                   
                    var traitAttrDeclaration = CreateTraitAttrDeclaration(
                        ExpressionGenerator.GenerateValueExpression("Category"), 
                        categoryArg.Expression, semanticModel);

                    dict.Add(testFixtureAttribute, traitAttrDeclaration);
                }
            }

            if (dict.Any())
            {
                //TODO: we should insert new attribute instead here and imiplement TestFixtureAttributeRemover instead
                //but I didn't find the way to insert several nodes in several different locations with a single call
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]).NormalizeWhitespace();
            }

            return root;
        }

        private AttributeSyntax CreateTraitAttrDeclaration(ExpressionSyntax key, ExpressionSyntax value, SemanticModel semanticModel)
        {
            //TODO: move to AttributeGenerator
            var arguments = new List<AttributeArgumentSyntax>
            {
                SyntaxFactory.AttributeArgument(key),
                SyntaxFactory.AttributeArgument(value)
            };

            var argumentList = SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(arguments));

            return SyntaxFactory.Attribute(SyntaxFactory.ParseName(typeof(TraitAttribute).FullName), argumentList).NormalizeWhitespace();
        }
    }
}