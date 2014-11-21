using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixture
{
    public class UseFixtureImplementor : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var @class in root.Classes())
            {
                var needsIUseFixtureImplementation = @class.HasTestFixtureSetUpMethods(semanticModel) 
                    || @class.HasTestFixtureTearDownMethods(semanticModel);

                if (needsIUseFixtureImplementation)
                {
                    var baseType = GetIUseFixtureDeclarationSyntax(@class);
                    var newMembers = new MemberDeclarationSyntax[]
                    {
                        GetTestDataPropertyDeclaration(@class),
                        GetSetFixtureMethodDeclaration(@class),
                    };

                    var newClass = @class
                        .AddBaseListTypes(baseType)
                        .AddMembers(newMembers)
                        .NormalizeWhitespace();

                    dict.Add(@class, newClass);
                }
            }

            if (dict.Any())
            {
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]).NormalizeWhitespace();
            }

            return root;
        }

        private static TypeSyntax GetIUseFixtureDeclarationSyntax(ClassDeclarationSyntax @class)
        {
            return SyntaxFactory.ParseTypeName(string.Format("Xunit.IUseFixture<{0}>", @class.Identifier.Text + "Data"));
        }

        private static PropertyDeclarationSyntax GetTestDataPropertyDeclaration(ClassDeclarationSyntax @class)
        {
            return
                SyntaxFactory.PropertyDeclaration(SyntaxFactory.IdentifierName(@class.Identifier.Text + "Data"),
                    SyntaxFactory.Identifier(@"TestData"))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithAccessorList(
                        SyntaxFactory.AccessorList(
                            SyntaxFactory.List<AccessorDeclarationSyntax>(new AccessorDeclarationSyntax[]
                            {
                                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                            })));
        }
    
        private static MethodDeclarationSyntax GetSetFixtureMethodDeclaration(ClassDeclarationSyntax @class)
        {
            return
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    SyntaxFactory.Identifier(@"SetFixture"))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        SyntaxFactory.ParameterList(
                            SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier(@"data"))
                                    .WithType(SyntaxFactory.IdentifierName(@class.Identifier.Text + "Data")))))
                    .WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.SingletonList<StatementSyntax>(
                                SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression,
                                        SyntaxFactory.IdentifierName(@"TestData"), SyntaxFactory.IdentifierName(@"data"))))));
        }
    }
}