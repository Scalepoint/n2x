using System.Collections.Generic;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.TestFixtureAttribute
{
    public class TestFixtureAttributeConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new TraitCategoryAdder();
            yield return new ClassAttributeRemover<NUnit.Framework.TestFixtureAttribute>();
            yield return new EmptyClassAttributeListRemover();
        }
    }
}