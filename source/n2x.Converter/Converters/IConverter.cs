using Microsoft.CodeAnalysis;

namespace n2x.Converter.Converters
{
    public interface IConverter
    {
        SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel);
    }
}