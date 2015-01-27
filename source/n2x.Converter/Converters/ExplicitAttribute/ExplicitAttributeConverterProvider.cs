using System.Collections.Generic;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.ExplicitAttribute
{
    public class ExplicitAttributeConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new ClassExplicitAttributeReplacer();
            yield return new ClassAttributeRemover<NUnit.Framework.ExplicitAttribute>();
            yield return new EmptyClassAttributeListRemover();

            yield return new MethodExplicitAttributeReplacer();
            yield return new MethodAttributeRemover<NUnit.Framework.ExplicitAttribute>();
            yield return new EmptyMethodAttributeListRemover();
        }
    }
}