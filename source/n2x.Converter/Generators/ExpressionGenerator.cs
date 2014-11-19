using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Generators
{
    public class ExpressionGenerator
    {
        public static ExpressionSyntax GenerateValueExpression(string value)
        {
            var valueString = SymbolDisplay.FormatLiteral(value, quote: true);
            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(valueString, value));
        }
    }
}