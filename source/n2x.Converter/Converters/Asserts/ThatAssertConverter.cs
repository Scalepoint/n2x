using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Generators;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Asserts
{
    public class ThatAssertConverter : IConverter
    {
        private readonly IDictionary<string, string> _oneParameterExpressionTransformations = new Dictionary<string, string>
        {
            {"Is.Null", "Null"},
            {"Is.Not.Null", "NotNull"},
            {"Is.True", "True"},
            {"Is.Empty", "Empty"},
            {"Is.Not.Empty", "NotEmpty"},
            {"Is.False", "False"},
        };

        private readonly IDictionary<string, string> _twoParametersExpressionTransformations = new Dictionary<string, string>
        {
            {"Is.EqualTo", "Equal"},
            {"Is.Not.EqualTo", "NotEqual"},
            {"Is.SameAs", "Same"},
            {"Is.Not.SameAs", "NotSame"},
        };

        private readonly IDictionary<string, string> _twoParametersReverseExpressionTransformations = new Dictionary<string, string>
        {
            {"Is.StringContaining", "Contains"},
            {"Is.Not.StringContaining", "DoesNotContain"},
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

                    if (!symbol.IsNunitAssert())
                    {
                        continue;
                    }


                    var methodName = symbol.Name;

                    if (methodName != "That")
                    {
                        continue;
                    }

                    ArgumentListSyntax methodArguments = invocationExpressionSyntax.ArgumentList;
                    var actualArgument = methodArguments.Arguments.First();

                    if (methodArguments.Arguments.Count == 1 
                        || (methodArguments.Arguments.Count == 2 && IsString(methodArguments.Arguments[1])))
                    {
                        var newExpression = ExpressionGenerator.CreateAssertExpression("True", actualArgument);

                        dict.Add(expressionStatementSyntax, newExpression);
                    }
                    else
                    {
                        var isArgument = methodArguments.Arguments[1];
                        var isArgumentString = GetStringExpression(isArgument);

                        if (isArgumentString == null)
                        {
                            continue;
                        }

                        string newMethodName;
                        if (_oneParameterExpressionTransformations.TryGetValue(isArgumentString, out newMethodName))
                        {
                            var newExpression = ExpressionGenerator.CreateAssertExpression(newMethodName, actualArgument);

                            dict.Add(expressionStatementSyntax, newExpression);
                        }
                        else if (_twoParametersExpressionTransformations.TryGetValue(isArgumentString, out newMethodName))
                        {
                            var expectedArgument = GetFirstArgument(isArgument);

                            var newExpression = ExpressionGenerator.CreateAssertExpression(newMethodName, actualArgument,
                                expectedArgument);
                            dict.Add(expressionStatementSyntax, newExpression);
                        }
                        else if (_twoParametersReverseExpressionTransformations.TryGetValue(isArgumentString, out newMethodName))
                        {
                            var expectedArgument = GetFirstArgument(isArgument);

                            var newExpression = ExpressionGenerator.CreateAssertExpression(newMethodName,
                                expectedArgument, actualArgument);
                            dict.Add(expressionStatementSyntax, newExpression);
                        }
                    }
                }
            }

            return root.ReplaceNodes(dict);
        }

        private bool IsString(ArgumentSyntax argumentSyntax)
        {
            var literalExpressionSyntax = argumentSyntax.Expression as LiteralExpressionSyntax;
            return literalExpressionSyntax != null;
        }

        private static string GetStringExpression(ArgumentSyntax argument)
        {
            var invocationExpressionSyntax = argument.Expression as InvocationExpressionSyntax;
            if (invocationExpressionSyntax != null)
            {
                return invocationExpressionSyntax.Expression.ToString();
            }

            var memberAccessExpressionSyntax = argument.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax != null)
            {
                return memberAccessExpressionSyntax.ToString();
            }

            return null;
        }

        private static ArgumentSyntax GetFirstArgument(ArgumentSyntax argument)
        {
            return ((InvocationExpressionSyntax)argument.Expression).ArgumentList.Arguments.First();
        }
    }
}