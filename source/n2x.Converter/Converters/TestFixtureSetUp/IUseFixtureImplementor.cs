using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class IUseFixtureImplementor : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var result = root;
            var classes = result.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var @class in classes)
            {
                var hasFixtureSetUpMethod = @class.Members.OfType<MethodDeclarationSyntax>()
                    .Any(m => m.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .Any(a => ModelExtensions.GetTypeInfo(semanticModel, a).Type.IsTestFixtureSetUpAttribute()));

                if (hasFixtureSetUpMethod)
                {
                    var baseTypes = SyntaxFactory.SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]
                    {
                        GetIUseFixtureDeclarationSyntax(@class)
                    });

                    if (@class.BaseList != null)
                    {
                        baseTypes.AddRange(@class.BaseList.Types);
                    }

                    var newMembers = new MemberDeclarationSyntax[]
                    {
                        GetTestDataPropertyDeclaration(@class),
                        GetSetFixtureMethodDeclaration(@class),
                    };

                    var newClass = @class
                        .WithBaseList(SyntaxFactory.BaseList(baseTypes))
                        .AddMembers(newMembers);
                    dict.Add(@class, newClass);
                }
            }

            if (dict.Any())
            {
                return result.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]);
            }

            return result;
        }

        private static NameSyntax GetIUseFixtureDeclarationSyntax(ClassDeclarationSyntax @class)
        {
            return SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(@"Xunit"), 
                SyntaxFactory.GenericName(SyntaxFactory.Identifier(@"IUseFixture"))
                    .WithTypeArgumentList(
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                            SyntaxFactory.IdentifierName(@class.Identifier.Text + "Data")))));
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