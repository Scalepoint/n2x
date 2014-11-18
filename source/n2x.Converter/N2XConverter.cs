using Microsoft.CodeAnalysis;

namespace n2x.Converter
{
    public class N2XConverter : IN2XConverter
    {
        private readonly IConverterProvider _converterProvider;

        public N2XConverter(IConverterProvider converterProvider)
        {
            _converterProvider = converterProvider;
        }

        public void Convert(Project project)
        {
            throw new System.NotImplementedException();
        }
    }
}