using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestDataClassCreator();
            yield return new UseFixtureImplementor();
            yield return new TestFixtureSetUpMethodMover();
            yield return new MethodRemover<NUnit.Framework.TestFixtureSetUpAttribute>();
            yield return new TestFixtureTearDownMethodMover();
            yield return new MethodRemover<NUnit.Framework.TestFixtureTearDownAttribute>();
        }
    }
}