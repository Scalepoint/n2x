using Microsoft.CodeAnalysis;

namespace n2x.Converter.Converters
{
    public interface IDocumentConverter
    {
        Document Convert(Document document);
    }
}