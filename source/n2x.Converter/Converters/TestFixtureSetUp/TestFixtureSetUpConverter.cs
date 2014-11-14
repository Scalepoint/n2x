using System.Collections.Generic;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class TestFixtureSetUpConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestDataClassCreator();
            yield return new IUseFixtureImplementor();
            yield return new SetUpMethodMover();
            yield return new SetUpMethodRemover();
        }
    }
}