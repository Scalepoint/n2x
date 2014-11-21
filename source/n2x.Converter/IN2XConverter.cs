using System;
using Microsoft.CodeAnalysis;

namespace n2x.Converter
{
    public interface IN2XConverter
    {
        bool ConvertSolution(string solutionFilePath, Action<int, int> progress = null);
    }
}