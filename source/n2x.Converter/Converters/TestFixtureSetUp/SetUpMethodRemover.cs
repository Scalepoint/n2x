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
            var result = root;
            var classes = result.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var fixtureSetUpMethods = new List<MethodDeclarationSyntax>();
            foreach (var @class in classes)
            {
                var methods = @class.Members.OfType<MethodDeclarationSyntax>()
                    .Where(m => m.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .Any(a => ModelExtensions.GetTypeInfo(semanticModel, a).Type.IsTestFixtureSetUpAttribute()))
                    .ToList();

                fixtureSetUpMethods.AddRange(methods);
            }

            if (fixtureSetUpMethods.Any())
            {
                return result.RemoveNodes(fixtureSetUpMethods, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return result;
        }
    }
}