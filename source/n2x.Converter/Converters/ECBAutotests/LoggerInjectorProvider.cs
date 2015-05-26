using System.Collections.Generic;

namespace n2x.Converter.Converters.ECBAutotests
{
    public class LoggerInjectorProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new LoggerInjector();
        }
    }
}