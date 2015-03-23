using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureTearDownMethodMover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var useFixtureClass in root.GetIClassFixtureClasses(semanticModel))
            {
                var testDataClassName = useFixtureClass.GetIClassFixtureTypeArgumentName(semanticModel);
                var testDataClass = root.Classes().SingleOrDefault(c => c.Identifier.Text == testDataClassName);
                var fixtureTearDownMethod = useFixtureClass.GetTestFixtureTearDownMethods(semanticModel).FirstOrDefault();

                if (fixtureTearDownMethod != null && testDataClass != null)
                {
                    var disposeMethod = GetDisposeMethodDeclaration(fixtureTearDownMethod);
                    var modifiedTestDataClass = testDataClass
                        .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("System.IDisposable")))
                        .AddMembers(disposeMethod)
                        .NormalizeWhitespace();

                    dict.Add(testDataClass, modifiedTestDataClass);
                }
            }

            return root.ReplaceNodes(dict);
        }

        private MemberDeclarationSyntax GetDisposeMethodDeclaration(MethodDeclarationSyntax fixtureTearDownMethod)
        {
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Dispose")
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(fixtureTearDownMethod.Body);
        }
    }
}