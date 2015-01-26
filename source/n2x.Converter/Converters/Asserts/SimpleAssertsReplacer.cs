using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Generators;
using n2x.Converter.Utils;

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

                    var expressionString = invocationExpressionSyntax.Expression.ToString();
                    ArgumentListSyntax methodArguments = invocationExpressionSyntax.ArgumentList;

                    if (!expressionString.StartsWith("Assert."))
                    {
                        continue;
                    }

                    var methodName = expressionString.Split('.')[1];
                    if (_assertMethodTransformations.ContainsKey(methodName))
                    {
                        var newMethodName = _assertMethodTransformations[methodName];
                        var newExpression = ExpressionGenerator.CreateAssertExpression(newMethodName, methodArguments);

                        dict.Add(expressionStatementSyntax, newExpression);
                    }
                }
            }

            if (dict.Any())
            {
                return root
                    .ReplaceNodes(dict.Keys, (n1, n2) => dict[n1])
                    .NormalizeWhitespace();
            }

            return root;
        }
    }
}