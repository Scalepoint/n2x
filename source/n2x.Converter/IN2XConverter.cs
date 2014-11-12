using Microsoft.CodeAnalysis;

namespace n2x.Converter
{
    public interface IN2XConverter
    {
        void Convert(Project project);
    }
}