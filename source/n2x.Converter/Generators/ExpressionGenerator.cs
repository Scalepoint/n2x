using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Generators
{
    public class ExpressionGenerator
    {
        public static ExpressionSyntax GenerateValueExpression(string value, bool quote = true)
        {
            var valueString = SymbolDisplay.FormatLiteral(value, quote);
            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(valueString, value));
        }

        public static AttributeSyntax GenerateAttribute<T>(params AttributeArgumentSyntax[] args)
            where T : Attribute
        {
            var arguments = new List<AttributeArgumentSyntax>(args);
            var argumentList = SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(arguments));

            var attributeType = SyntaxFactory.ParseName(typeof (T).FullName);
            var attribute = arguments.Any() ? SyntaxFactory.Attribute(attributeType, argumentList) : SyntaxFactory.Attribute(attributeType);

            return attribute.NormalizeWhitespace();
        }

        public static ExpressionStatementSyntax CreateAssertExpression(string methodName, ArgumentSyntax firstArgument, ArgumentSyntax secondArgument = null)
        {
            var expressionString = secondArgument == null
                ? string.Format("Xunit.Assert.{0}({1})", methodName, firstArgument)
                : string.Format("Xunit.Assert.{0}({1}, {2})", methodName, firstArgument, secondArgument);
            return SyntaxFactory.ExpressionStatement(SyntaxFactory.ParseExpression(expressionString));
        }

        public static InvocationExpressionSyntax CreateAssertInvocation(string methodName, IEnumerable<TypeSyntax> typeArguments, ArgumentListSyntax arguments)
        {
            SimpleNameSyntax operatorToken = SyntaxFactory.IdentifierName(methodName);
            if (typeArguments.Any())
            {
                operatorToken = SyntaxFactory.GenericName(methodName)
                    .WithTypeArgumentList(
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(typeArguments)));
            }

            return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.ParseName("Xunit"),
                        SyntaxFactory.IdentifierName("Assert")),
                    operatorToken),
                arguments);
        }

        public static UsingDirectiveSyntax GenerateXunitUsing()
        {
            return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Xunit"));
        }

    }
}