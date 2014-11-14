using Microsoft.CodeAnalysis;

namespace n2x.Converter.Utils
{
    public static class XUnitTypeSymbolExtensions
    {
        public static bool IsIUseFixtureInterface(this ITypeSymbol typeSymbol)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString().StartsWith(@"Xunit.IUseFixture<");
        }

        public static bool IsIUseFixtureInterfaceOf(this ITypeSymbol typeSymbol, string arg)
        {
            return !typeSymbol.IsErrorType() &&
                typeSymbol.ToDisplayString() == string.Format("Xunit.IUseFixture<{0}>", arg);
        }
    }
}