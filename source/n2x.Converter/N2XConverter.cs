using Microsoft.CodeAnalysis;

namespace n2x.Converter
{
    public class N2XConverter : IN2XConverter
    {
        private readonly IDocumentConverterProvider _converterProvider;

        public N2XConverter(IDocumentConverterProvider converterProvider)
        {
            _converterProvider = converterProvider;
        }

        public void Convert(Project project)
        {
            throw new System.NotImplementedException();
        }
    }
}