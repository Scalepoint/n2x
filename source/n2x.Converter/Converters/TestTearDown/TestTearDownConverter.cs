using System.Collections.Generic;

namespace n2x.Converter.Converters.TestTearDown
{
    public class TestTearDownConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new DisposableInterfaceImplementer();
            yield return new TearDownMethodMover();
            yield return new TearDownMethodRemover();
        }
    }
}
