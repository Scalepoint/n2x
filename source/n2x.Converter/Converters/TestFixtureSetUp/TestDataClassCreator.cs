using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class TestDataClassCreator : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var result = root;

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var testDataClasses = new List<ClassDeclarationSyntax>();
            foreach (var @class in classes)
            {
                var hasFixtureSetUpMethod = @class.Members.OfType<MethodDeclarationSyntax>()
                    .Any(m => m.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .Any(a => ModelExtensions.GetTypeInfo(semanticModel, a).Type.IsTestFixtureSetUpAttribute()));

                if (hasFixtureSetUpMethod)
                {
                    var testDataClass = CreateTestDataClassDeclaration(@class);

                    testDataClasses.Add(testDataClass);
                }
            }

            if (testDataClasses.Any())
            {
                var firstClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                return result.InsertNodesBefore(firstClass, testDataClasses);
            }

            return result;
        }

        private static ClassDeclarationSyntax CreateTestDataClassDeclaration(ClassDeclarationSyntax @class)
        {
            return SyntaxFactory.ClassDeclaration(@class.Identifier.ValueText + "Data")
                .WithModifiers(@class.Modifiers)
                .NormalizeWhitespace();
        }
    }
}