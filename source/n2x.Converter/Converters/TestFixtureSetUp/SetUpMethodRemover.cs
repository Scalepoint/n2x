using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class SetUpMethodRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var testSetUpMethods = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .SelectMany(c => c.GetTestSetUpMethods(semanticModel))
                .ToList();

            if (testSetUpMethods.Any())
            {
                return root.RemoveNodes(testSetUpMethods, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
    }
}