using System;
using System.Collections.Generic;

namespace n2x.Converter.Converters.Asserts
{
    public class AssertsConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new SimpleAssertsReplacer();
            yield return new ThatAssertConverter();
        }
    }
}
