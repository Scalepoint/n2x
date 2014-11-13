using System.Collections.Generic;
using n2x.Converter.Converters;

namespace n2x.Converter
{
    public class DocumentConverterProvider : IDocumentConverterProvider
    {
        public DocumentConverterProvider()
        {
            DocumentConverters = new List<IConverter>();
        }

        public IList<IConverter> DocumentConverters { get; private set; }
    }
}