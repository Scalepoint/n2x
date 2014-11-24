using System.Collections.Generic;
using n2x.Converter.Converters;
using n2x.Converter.Converters.TestFixture;
using n2x.Converter.Converters.TestFixtureAttribute;
using n2x.Converter.Converters.TestTearDown;
using n2x.Converter.Converters.TestAttribute;

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
                new TestTearDownConverter(),
                new TestAttributeConverter()
            };
        }

        public IList<IDocumentConverter> Converters { get; }
    }
}