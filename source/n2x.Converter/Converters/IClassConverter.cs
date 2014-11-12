using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Converters
{
    public interface IClassConverter
    {
        SyntaxNode Convert(ClassDeclarationSyntax @class);
    }
}