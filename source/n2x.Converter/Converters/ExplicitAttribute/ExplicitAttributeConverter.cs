using System.Collections.Generic;

namespace n2x.Converter.Converters.ExplicitAttribute
{
    public class ExplicitAttributeConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new ExplicitAttributeReplacer();
        }
    }
}