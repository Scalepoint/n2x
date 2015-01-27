using System.Collections.Generic;
using System.Linq;
using n2x.Converter.Converters.Common;
using NUnit.Framework;

namespace n2x.Converter.Converters.SetUp
{
    public class SetUpConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new SetUpMethodMover();
            yield return new MethodRemover<SetUpAttribute>();
        }
    }
}