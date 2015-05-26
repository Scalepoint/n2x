using System;
using Microsoft.CodeAnalysis;

namespace n2x.Converter.Utils
{
    public static class SymbolExtensions
    {
        public static bool IsOfType<T>(this ISymbol symbol)
        {
            if (symbol.ContainingType != null)
            {
                return symbol.ContainingType.ToDisplayString() == typeof(T).FullName;
            }

            return symbol.ToDisplayString() == typeof(T).FullName;
        }

        [Obsolete]
        public static bool IsOfTypeWithNameLike(this ISymbol symbol, string typeName)
        {
            if (symbol.ContainingType != null)
            {
                return symbol.ContainingType.ToDisplayString().Contains(typeName);
            }

            return symbol.ToDisplayString().Contains(typeName);
        }
    }
}