using System.Collections.Generic;

namespace n2x.Converter.Converters.CategoryAttribute
{
    public class CategoryAttributeConverterProvider : IConverterProvider
    {
        public IEnumerable<IConverter> GetConverters()
        {
            yield return new OnClassCategoryAttributeReplacer();
            yield return new OnMethodCategoryAttributeReplacer();
        }
    }
}