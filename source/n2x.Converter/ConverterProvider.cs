using System.Collections.Generic;
using n2x.Converter.Converters;

namespace n2x.Converter
{
    public class ConverterProvider : IConverterProvider
    {
        public ConverterProvider()
        {
            Converters = new List<IConverter>();
        }

        public IList<IConverter> Converters { get; private set; }
    }
}