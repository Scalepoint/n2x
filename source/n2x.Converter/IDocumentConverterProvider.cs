using System.Collections;
using System.Collections.Generic;
using n2x.Converter.Converters;

namespace n2x.Converter
{
    public interface IDocumentConverterProvider
    {
        IList<IDocumentConverter> Converters { get; }
    }
}