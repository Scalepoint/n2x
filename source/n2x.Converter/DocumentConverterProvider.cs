using System.Collections.Generic;
using n2x.Converter.Converters;
using n2x.Converter.Converters.TestFixture;
using n2x.Converter.Converters.TestFixtureAttribute;

namespace n2x.Converter
{
    public class DocumentConverterProvider : IDocumentConverterProvider
    {
        public DocumentConverterProvider()
        {
            Converters = new List<IDocumentConverter>
            {
                new TestFixtureConverter(),
                new TestFixtureAttributeConverter(),
            };
        }

        public IList<IDocumentConverter> Converters { get; }
    }
}