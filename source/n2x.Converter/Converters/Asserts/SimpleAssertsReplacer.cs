﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            {"AreEqual", "Equal(expected,actual)" },
            {"AreNotEqual", "NotEqual(expected,actual)"},
            {"AreNotSame", "NotSame(expected,actual)"},
            {"AreSame", "Same(expected,actual)"},
            {"Contains", "Contains(expected,actual)"},
            {"IsAssignableFrom", "IsAssignableFrom"},
            {"IsEmpty", "Empty"},
            {"IsFalse", "False(condition)" },
            {"IsInstanceOfType", "IsType"},
            {"IsNotEmpty", "NotEmpty"},
            {"IsNotInstanceOfType", "IsNotType"},
            {"IsNotNull", "NotNull(anObject)" },
            {"IsNull", "Null(anObject)" },
            {"IsTrue", "True(condition)" },
        };

        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var assertInvocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>()
                .Where(invocation => invocation.IsNunitAssert(semanticModel));

            foreach (var assertInvocation in assertInvocations)
            {
                var symbol = ModelExtensions.GetSymbolInfo(semanticModel, assertInvocation).Symbol;
                string newMethodPattern;
                if (_assertMethodTransformations.TryGetValue(symbol.Name, out newMethodPattern))
                {
                    var newMethodName = GetMethodNameFromPattern(newMethodPattern);
                    var methodArguments = GetMethodArgsByPattern(newMethodPattern, assertInvocation, semanticModel);
                    var newExpression = ExpressionGenerator.CreateAssertInvocation(newMethodName, methodArguments);
                    dict.Add(assertInvocation, newExpression);
                }
            }

            return root.ReplaceNodes(dict);
        }

        private ArgumentListSyntax GetMethodArgsByPattern(string pattern, InvocationExpressionSyntax assertInvocation, SemanticModel semanticModel)
        {
            var i = pattern.IndexOf("(", StringComparison.Ordinal);
            if (i == -1)
            {
                return assertInvocation.ArgumentList;
            }

            var symbol = (IMethodSymbol)semanticModel.GetSymbolInfo(assertInvocation).Symbol;

            var argsPattern = pattern.Substring(i + 1, pattern.Length - i - 2);
            var result = argsPattern.Split(',')
                .Select(patternName => symbol.Parameters.Single(p => p.Name == patternName))
                .Select(param => assertInvocation.ArgumentList.Arguments.ElementAt(param.Ordinal))
                .ToList();

            return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(result));
        }

        private string GetMethodNameFromPattern(string pattern)
        {
            var i = pattern.IndexOf("(", StringComparison.Ordinal);
            if (i == -1)
            {
                return pattern;
            }

            return pattern.Substring(0, i);
        }
    }
}