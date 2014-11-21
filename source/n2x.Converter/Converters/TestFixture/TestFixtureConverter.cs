using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestDataClassCreator();
            yield return new UseFixtureImplementor();
            yield return new TestFixtureSetUpMethodMover();
            yield return new TestFixtureSetUpMethodRemover();
            yield return new TestFixtureTearDownMethodMover();
            yield return new TestFixtureTearDownMethodRemover();
        }
    }
}