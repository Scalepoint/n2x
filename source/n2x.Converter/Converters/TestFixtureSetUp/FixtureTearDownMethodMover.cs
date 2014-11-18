using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class FixtureTearDownMethodMover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var useFixtureClass in root.GetIUseFixtureClasses(semanticModel))
            {
                var testDataClassName = useFixtureClass.GetIUseFixtureTypeArgumentName(semanticModel);
                var testDataClass = root.Classes().SingleOrDefault(c => c.Identifier.Text == testDataClassName);
                var fixtureTearDownMethod = useFixtureClass.GetTestFixtureTearDownMethods(semanticModel).FirstOrDefault();

                if (fixtureTearDownMethod != null && testDataClass != null)
                {
                    var disposeMethod = GetDisposeMethodDeclaration(fixtureTearDownMethod);
                    var modifiedTestDataClass = testDataClass
                        .AddBaseListTypes(SyntaxFactory.ParseTypeName("IDisposable"))
                        .AddMembers(disposeMethod);

                    dict.Add(testDataClass, modifiedTestDataClass);
                }
            }

            if (dict.Any())
            {
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]);
            }

            return root;
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