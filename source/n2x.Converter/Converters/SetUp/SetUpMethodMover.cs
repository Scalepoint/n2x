using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.SetUp
{
    public class SetUpMethodMover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var @class in root.Classes().WithSetUpMethods(semanticModel))
            {
                var setUpMethod = @class.GetSetUpMethods(semanticModel).First();

                var defaultConstructor = @class.Members
                    .OfType<ConstructorDeclarationSyntax>()
                    .SingleOrDefault(c => c.ParameterList == null || !c.ParameterList.Parameters.Any());

                if (defaultConstructor != null)
                {
                    var newDefaultConstructor = defaultConstructor.AddBodyStatements(setUpMethod.Body.Statements.ToArray());
                    dict.Add(defaultConstructor, newDefaultConstructor);
                }
                else
                {
                    defaultConstructor = GetDefaultConstructorDeclaration(@class, setUpMethod);
                    var modifiedTestClass = @class.AddMembers(defaultConstructor);
                    dict.Add(@class, modifiedTestClass);
                }
            }

            if (dict.Any())
            {
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]).NormalizeWhitespace();
            }

            return root;
        }

        private ConstructorDeclarationSyntax GetDefaultConstructorDeclaration(ClassDeclarationSyntax @class, MethodDeclarationSyntax setUpMethod)
        {
            return SyntaxFactory.ConstructorDeclaration(@class.Identifier)
                .WithModifiers(SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(setUpMethod.Body);
        }
    }
}