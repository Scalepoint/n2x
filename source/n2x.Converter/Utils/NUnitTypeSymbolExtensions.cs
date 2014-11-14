using Microsoft.CodeAnalysis;

namespace n2x.Converter.Utils
{
    public static class NUnitTypeSymbolExtensions
    {
        public static bool IsTestFixtureSetUpAttribute(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == "NUnit.Framework.TestFixtureSetUpAttribute";
        }
    }
}