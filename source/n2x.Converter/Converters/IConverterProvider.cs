using System.Collections.Generic;

namespace n2x.Converter.Converters
{
    public interface IConverterProvider
    {
        IEnumerable<IConverter> GetConverters();
    }
}