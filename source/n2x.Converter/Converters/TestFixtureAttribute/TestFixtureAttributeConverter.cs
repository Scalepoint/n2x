using System.Collections.Generic;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.TestFixtureAttribute
{
    public class TestFixtureAttributeConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TraitCategoryAdder();
            yield return new ClassAttributeRemover<NUnit.Framework.TestFixtureAttribute>();
            yield return new EmptyClassAttributeListRemover();
        }
    }
}