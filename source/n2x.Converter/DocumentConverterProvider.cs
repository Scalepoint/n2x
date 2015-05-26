using System.Collections.Generic;
using n2x.Converter.Converters;
using n2x.Converter.Converters.Asserts;
using n2x.Converter.Converters.CategoryAttribute;
using n2x.Converter.Converters.ECBAutotests;
using n2x.Converter.Converters.ExplicitAttribute;
using n2x.Converter.Converters.MoveToPublic;
using n2x.Converter.Converters.SetUp;
using n2x.Converter.Converters.TestFixture;
using n2x.Converter.Converters.TestFixtureAttribute;
using n2x.Converter.Converters.TearDownAttribute;
using n2x.Converter.Converters.TestAttribute;
using n2x.Converter.Converters.TestCaseAttribute;
using n2x.Converter.Converters.TestOutputHelperInjector;
using n2x.Converter.Converters.Using;

namespace n2x.Converter
{
    public class DocumentConverterProvider : IDocumentConverterProvider
    {
        public DocumentConverterProvider()
        {
            Converters = new List<IDocumentConverter>
            {
                //new DocumentConverter(new TestFixtureConverterProvider()),
                //new DocumentConverter(new TestFixtureAttributeConverterProvider()),
                //new DocumentConverter(new SetUpConverterProvider()),
                //new DocumentConverter(new TearDownAttributeConverterProvider()),
                //new DocumentConverter(new TestAttributeConverterProvider()),
                //new DocumentConverter(new TestCaseAttributeConverterProvider()),
                //new DocumentConverter(new ExplicitAttributeConverterProvider()),
                //new DocumentConverter(new CategoryAttributeConverterProvider()),
                //new DocumentConverter(new AssertsConverterProvider()),
                //new DocumentConverter(new MoveToPublicProvider()),
                //new DocumentConverter(new TestOutputHelperInjectorProvider()),
                //new DocumentConverter(new UsingConverterProvider()),
                new DocumentConverter(new LoggerInjectorProvider()),
            };
        }

        public IList<IDocumentConverter> Converters { get; }
    }
}