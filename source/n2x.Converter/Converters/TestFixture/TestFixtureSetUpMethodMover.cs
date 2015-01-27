using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureSetUpMethodMover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var useFixtureClass in root.GetIUseFixtureClasses(semanticModel))
            {
                var testDataClassName = useFixtureClass.GetIUseFixtureTypeArgumentName(semanticModel);
                var testDataClass = root.Classes().SingleOrDefault(c => c.Identifier.Text == testDataClassName);
                var fixtureSetUpMethod = useFixtureClass.GetTestFixtureSetUpMethods(semanticModel).FirstOrDefault();

                if (fixtureSetUpMethod != null && testDataClass != null)
                {
                    var ctor = GetCtorDeclaration(testDataClass, fixtureSetUpMethod);
                    var modifiedTestDataClass = testDataClass.AddMembers(ctor).NormalizeWhitespace();

                    dict.Add(testDataClass, modifiedTestDataClass);
                }
            }

            return root.ReplaceNodes(dict);
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