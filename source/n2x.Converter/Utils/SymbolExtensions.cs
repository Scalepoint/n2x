using Microsoft.CodeAnalysis;

namespace n2x.Converter.Utils
{
    public static class SymbolExtensions
    {
        public static bool IsOfType<T>(this ISymbol symbol)
        {
            return symbol.ContainingType.ToDisplayString() == typeof(T).FullName;
        }
    }
}