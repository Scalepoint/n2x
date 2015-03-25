using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.MoveToPublic
{
    public class ToPublicMover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            var nonPublicTestClasses = root.Classes()
                .Where(c => c.IsXUnitTestClass(semanticModel))
                .Where(c => !c.IsPublic());

            foreach (var @class in nonPublicTestClasses)
            {
                var cleanedOriginalModifiers = @class.Modifiers
                    .Where(m => !m.IsKind(SyntaxKind.InternalKeyword))
                    .Where(m => !m.IsKind(SyntaxKind.PrivateKeyword))
                    .Where(m => !m.IsKind(SyntaxKind.ProtectedKeyword));

                var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
                var newModifiers = SyntaxFactory.TokenList(publicModifier)
                    .AddRange(cleanedOriginalModifiers);

                var originalLeadingTrivia = @class.GetLeadingTrivia(); //preserve original leading trivia
                var newClass = @class
                    .WithLeadingTrivia()
                    .WithModifiers(newModifiers)
                    .WithLeadingTrivia(originalLeadingTrivia);

                dict.Add(@class, newClass);
            }

            return root.ReplaceNodes(dict);
        }
    }
}