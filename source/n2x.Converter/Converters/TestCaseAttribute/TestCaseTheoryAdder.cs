using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit;
using Xunit.Extensions;

namespace n2x.Converter.Converters.TestCaseAttribute
{
    public class TestCaseTheoryAdder : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var methods = root.Classes().SelectMany(p => p.GetTestCaseMethods(semanticModel));

            foreach (var method in methods)
            {
                var testCaseAttributes = method.GetAttributes<NUnit.Framework.TestCaseAttribute>(semanticModel);

                var theoryDeclaration = ExpressionGenerator.GenerateAttribute<TheoryAttribute>();
                var newMethod = method.AddAtribute(theoryDeclaration);

                foreach (var testCaseAttribute in testCaseAttributes)
                {
                    var arguments = testCaseAttribute
                        .ArgumentList
                        ?.Arguments
                        .Where(a => a.NameEquals == null || a.NameEquals.Name.Identifier.Text != "Category")
                        .ToArray();
                    var inlineDataDeclaration = ExpressionGenerator.GenerateAttribute<InlineDataAttribute>(arguments);
                    newMethod = newMethod.AddAtribute(inlineDataDeclaration);
                }

                dict.Add(method, newMethod);
            }

            return root.ReplaceNodes(dict);
        }
    }
}