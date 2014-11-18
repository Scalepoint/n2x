using System.Collections;
using System.Collections.Generic;
using n2x.Converter.Converters;

namespace n2x.Converter
{
    public interface IConverterProvider
    {
        IList<IConverter> Converters { get; }
    }
}