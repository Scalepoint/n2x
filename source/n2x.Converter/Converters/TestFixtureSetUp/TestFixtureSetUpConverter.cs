using System.Collections.Generic;

namespace n2x.Converter.Converters.TestFixtureSetUp
{
    public class TestFixtureSetUpConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new TestDataCreator();
            yield return new SetUpMethodRemover();
        }
    }
}