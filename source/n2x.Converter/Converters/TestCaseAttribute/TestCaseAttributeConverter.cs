using System.Collections.Generic;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.TestCaseAttribute
{
    public class TestCaseAttributeConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestCaseTheoryAdder();
            yield return new MethodAttributeRemover<NUnit.Framework.TestCaseAttribute>();
            yield return new EmptyMethodAttributeListRemover();
        }
    }
}