using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class TestDataCreator : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var result = root;

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var fixtureSetUpMethods = new List<MethodDeclarationSyntax>();
            var testDataClasses = new List<ClassDeclarationSyntax>();
            foreach (var @class in classes)
            {
                fixtureSetUpMethods = @class.Members.OfType<MethodDeclarationSyntax>()
                    .Where(m => m.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .Any(a => ModelExtensions.GetTypeInfo(semanticModel, a).Type.IsTestFixtureSetUpAttribute()))
                    .ToList();

                foreach (var method in fixtureSetUpMethods)
                {
                    var testDataClass = SyntaxFactory.ClassDeclaration(@class.Identifier.ValueText + "Data")
                        .WithModifiers(@class.Modifiers)
                        .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(new[] { method }))
                        .NormalizeWhitespace();

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
    }
}