using System;
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
            var result = root;
            var classes = result.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var testFixtureClasses = classes.Where(c => c.BaseList != null
                                                        && c.BaseList.Types
                                                            .Any(t => semanticModel.GetTypeInfo(t)
                                                                        .Type.IsIUseFixtureInterface())
                );

            foreach (var @class in testFixtureClasses)
            {
                var fixtureSetUpMethod = @class.Members.OfType<MethodDeclarationSyntax>()
                    .SingleOrDefault(m => m.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .Any(a => ModelExtensions.GetTypeInfo(semanticModel, a).Type.IsTestFixtureSetUpAttribute()));

                if (fixtureSetUpMethod != null)
                {
                    var testDataClassName = @class.BaseList.Types.Select(t => (INamedTypeSymbol)semanticModel.GetTypeInfo(t).Type)
                        .Single(m => m.IsIUseFixtureInterface())
                        .TypeArguments
                        .Single()
                        .Name;

                    var testDataClass = classes.Single(c => c.Identifier.Text == testDataClassName);


                    var newMembers = new MemberDeclarationSyntax[]
                    {
                        GetCtorDeclaration(testDataClass, fixtureSetUpMethod),
                    };

                    var newClass = testDataClass
                       .AddMembers(newMembers);

                    dict.Add(testDataClass, newClass);
                }
            }

            if (dict.Any())
            {
                return result.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]);
            }

            return result;
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