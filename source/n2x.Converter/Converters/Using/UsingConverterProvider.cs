using System.Collections.Generic;

namespace n2x.Converter.Converters.Using
{
    public class UsingConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new UsingReplacer();
        }
    }
}