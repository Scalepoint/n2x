using System.Collections.Generic;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class TestFixtureSetUpConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestDataClassCreator();
            yield return new UseFixtureImplementor();
            yield return new FixtureSetUpMethodMover();
            yield return new FixtureSetUpMethodRemover();
            yield return new FixtureTearDownMethodMover();
            yield return new FixtureTearDownMethodRemover();
        }
    }
}