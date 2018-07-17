using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using n2x.Converter.Converters.Common;

namespace n2x.Converter.Converters.TestFixture
{
    public class TestFixtureConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new TestDataClassCreator();
            yield return new UseFixtureImplementor();
            yield return new TestFixtureSetUpMethodMover();
#pragma warning disable CS0618 // Type or member is obsolete
            yield return new MethodRemover<NUnit.Framework.TestFixtureSetUpAttribute>();
#pragma warning restore CS0618 // Type or member is obsolete
            yield return new TestFixtureTearDownMethodMover();
#pragma warning disable CS0618 // Type or member is obsolete
            yield return new MethodRemover<NUnit.Framework.TestFixtureTearDownAttribute>();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}