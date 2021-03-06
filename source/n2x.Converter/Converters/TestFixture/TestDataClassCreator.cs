using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestDataClassCreator : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var newTestDataClasses = root
                .Classes()
                .Where(c => c.HasTestFixtureSetUpMethods(semanticModel) || c.HasTestFixtureTearDownMethods(semanticModel))
                .Select(CreateTestDataClassDeclaration)
                .ToList();

            if (newTestDataClasses.Any())
            {
                return root.InsertNodesBefore(root.FirstClass(), newTestDataClasses).NormalizeWhitespace();
            }

            return root;
        }

        private static ClassDeclarationSyntax CreateTestDataClassDeclaration(ClassDeclarationSyntax @class)
        {
            return SyntaxFactory.ClassDeclaration(@class.Identifier.ValueText + "Data")
                .WithModifiers(@class.Modifiers);
        }
    }
}