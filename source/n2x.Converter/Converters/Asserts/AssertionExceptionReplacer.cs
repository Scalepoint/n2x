using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Asserts
{
    public class AssertionExceptionReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            var catchFilters = root.DescendantNodes().OfType<CatchDeclarationSyntax>()
                .Where(d => d.Type.IsAssertionException(semanticModel));

            foreach (var catchFilter in catchFilters)
            {
                var xunitAssertExceptionType = SyntaxFactory.ParseTypeName("Xunit.Sdk.XunitException");
                var newDeclaration = SyntaxFactory.CatchDeclaration(catchFilter.OpenParenToken, 
                    xunitAssertExceptionType, 
                    catchFilter.Identifier, 
                    catchFilter.CloseParenToken);

                dict.Add(catchFilter, newDeclaration);
            }


            return root.ReplaceNodes(dict); ;
        }
    }
}