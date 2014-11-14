using Microsoft.CodeAnalysis;

namespace n2x.Converter.Utils
{
    public static class TypeSymbolExtensions
    {
        public static bool IsErrorType(this ITypeSymbol type)
        {
            return type.Kind == SymbolKind.ErrorType;
        }
    }
}