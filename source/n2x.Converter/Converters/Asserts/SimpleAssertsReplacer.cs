using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using NUnit.Framework;

namespace n2x.Converter.Converters.Asserts
{
    public class SimpleAssertsReplacer : IConverter
    {
        private readonly IDictionary<string, string> _assertMethodTransformations = new Dictionary<string, string>
        {
            {"AreEqual", "Equal"},
            {"AreNotEqual", "NotEqual"},
            {"AreNotSame", "NotSame"},
            {"AreSame", "Same"},
            {"Contains", "Contains"},
            {"IsAssignableFrom", "IsAssignableFrom"},
            {"IsEmpty", "Empty"},
            {"IsFalse", "False"},
            {"IsInstanceOfType", "IsType"},
            {"IsNotEmpty", "NotEmpty"},
            {"IsNotInstanceOfType", "IsNotType"},
            {"IsNotNull", "NotNull"},
            {"IsNull", "Null"},
            {"IsTrue", "True"},
        };

        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            var methods = root.Classes().SelectMany(p => p.Methods());
            foreach (var method in methods)
            {
                if (method.Body == null)
                {
                    continue;
                }

                foreach (StatementSyntax statementSyntax in method.Body.Statements)
                {
                    var expressionStatementSyntax = statementSyntax as ExpressionStatementSyntax;

                    var invocationExpressionSyntax = expressionStatementSyntax?.Expression as InvocationExpressionSyntax;
                    if (invocationExpressionSyntax == null)
                    {
                        continue;
                    }

                    var symbol = semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol;
                    if (symbol == null)
                    {
                        continue;
                    }

                    if (!symbol.IsNunitAssert())
                    {
                        continue;
                    }
                    var methodName = symbol.Name;
                    string newMethodName;
                    if (_assertMethodTransformations.TryGetValue(methodName, out newMethodName))
                    {
                        var newExpression = ExpressionGenerator.CreateAssertExpression(newMethodName, invocationExpressionSyntax.ArgumentList);

                        dict.Add(expressionStatementSyntax, newExpression);
                    }
                }
            }

            return root.ReplaceNodes(dict);
        }
    }
}