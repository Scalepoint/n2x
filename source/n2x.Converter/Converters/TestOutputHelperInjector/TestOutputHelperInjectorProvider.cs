using System.Collections.Generic;

namespace n2x.Converter.Converters.TestOutputHelperInjector
{
    public class TestOutputHelperInjectorProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new TestClassOutputHelperInjector();
        }
    }
}