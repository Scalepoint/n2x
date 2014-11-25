using System.Collections.Generic;
using System.Linq;

namespace n2x.Converter.Converters.SetUp
{
    public class SetUpConverter : DocumentConverter
    {
        protected override IEnumerable<IConverter> GetConverters()
        {
            yield return new SetUpMethodMover();
            yield return new SetUpMethodRemover();
        }
    }
}