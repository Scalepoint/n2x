using System.Collections.Generic;
using n2x.Converter.Converters;
using n2x.Converter.Converters.Asserts;
using n2x.Converter.Converters.ExplicitAttribute;
using n2x.Converter.Converters.SetUp;
using n2x.Converter.Converters.TestFixture;
using n2x.Converter.Converters.TestFixtureAttribute;
using n2x.Converter.Converters.TearDownAttribute;
using n2x.Converter.Converters.TestAttribute;
using n2x.Converter.Converters.TestCaseAttribute;

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
                new SetUpConverter(),
                new TearDownAttributeConverter(),
                new TestAttributeConverter(),
                new TestCaseAttributeConverter(),
                new ExplicitAttributeConverter(),
                new AssertsConverter(),
            };
        }

        public IList<IDocumentConverter> Converters { get; }
    }
}