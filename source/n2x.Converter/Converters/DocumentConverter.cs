using Microsoft.CodeAnalysis;

namespace n2x.Converter.Converters
{
    public class DocumentConverter : IDocumentConverter
    {
        private readonly IConverterProvider _converterProvider;

        public DocumentConverter(IConverterProvider converterProvider)
        {
            _converterProvider = converterProvider;
        }

        public virtual Document Convert(Document document)
        {
            var result = document;

            foreach (var converter in _converterProvider.GetConverters())
            {
                var root = result.GetSyntaxRootAsync().Result;
                var semanticModel = result.GetSemanticModelAsync().Result;

                var newRoot = converter.Convert(root, semanticModel);
                result = result.WithSyntaxRoot(newRoot);
            }

            return result;
        }
    }
}