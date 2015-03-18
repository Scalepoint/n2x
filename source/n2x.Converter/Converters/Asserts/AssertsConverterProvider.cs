using System;
using System.Collections.Generic;

namespace n2x.Converter.Converters.Asserts
{
    public class AssertsConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new SimpleAssertsReplacer();
            yield return new ThatAssertConverter();
            yield return new AssertionExceptionReplacer();
        }
    }
}
