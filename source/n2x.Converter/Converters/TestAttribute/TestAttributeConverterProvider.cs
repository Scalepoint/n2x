using System.Collections.Generic;

namespace n2x.Converter.Converters.TestAttribute
{
    public class TestAttributeConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new TestAttributeReplacer();
        }
    }
}