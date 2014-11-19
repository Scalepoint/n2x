using System.Collections.Generic;

namespace n2x.Converter.Converters.TestFixtureAttribute
{
    public class TestFixtureAttributeConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TraitCategoryAdder();
            yield return new TestFixtureAttributeRemover();
            yield return new EmptyAttributeListRemover();
        }
    }
}