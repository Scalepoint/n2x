using System.Collections.Generic;

namespace n2x.Converter.Converters.Using
{
    public class UsingConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new UsingReplacer();
        }
    }
}