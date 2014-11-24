using System.Collections.Generic;

namespace n2x.Converter.Converters.TestAttribute
{
    public class TestAttributeConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestAttributeReplacer();
        }
    }
}