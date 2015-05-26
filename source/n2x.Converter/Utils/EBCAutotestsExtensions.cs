using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Utils
{
    public static class EBCAutotestsExtensions
    {
        public static bool IsAncestorOf(this TypeSyntax type, string typeName, SemanticModel semanticModel)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(type);
            var symbol = symbolInfo.Symbol;
            if (symbol == null && symbolInfo.CandidateSymbols.Any())
            {
                symbol = symbolInfo.CandidateSymbols.First();
            }

            var typeSymbol = symbol as ITypeSymbol;
            if (typeSymbol == null)
            {
                return false;
            }

            var baseType = typeSymbol.BaseType;
            while (baseType != null)
            {
                if (baseType.IsOfTypeWithNameLike(typeName))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}