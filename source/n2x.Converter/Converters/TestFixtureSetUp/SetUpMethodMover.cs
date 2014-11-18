using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class SetUpMethodMover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var useFixtureClass in root.GetIUseFixtureClasses(semanticModel))
            {
                var testDataClassName = useFixtureClass.GetIUseFixtureTypeArgumentName(semanticModel);
                var testDataClass = root.Classes().SingleOrDefault(c => c.Identifier.Text == testDataClassName);
                var fixtureSetUpMethod = useFixtureClass.GetTestSetUpMethods(semanticModel).FirstOrDefault();

                if (fixtureSetUpMethod != null && testDataClass != null)
                {
                    var ctor = GetCtorDeclaration(testDataClass, fixtureSetUpMethod);
                    var modifiedTestDataClass = testDataClass.AddMembers(ctor);

                    dict.Add(testDataClass, modifiedTestDataClass);
                }
            }

            if (dict.Any())
            {
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]);
            }

            return root;
        }

        private ConstructorDeclarationSyntax GetCtorDeclaration(ClassDeclarationSyntax @class, MethodDeclarationSyntax fixtureSetUpMethod)
        {
            return SyntaxFactory.ConstructorDeclaration(@class.Identifier)
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(fixtureSetUpMethod.Body);
        }
    }
}