using System.Collections.Generic;

namespace n2x.Converter.Converters.MoveToPublic
{
    public class MoveToPublicProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new ToPublicMover();
        }
    }
}