using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace n2x.Converter.Utils
{
    public static class XunitExtensions
    {
        public static bool HasXunitFactMethod(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetXunitFactMethod(semanticModel).Any();
        }

        public static bool HasXunitTheoryMethod(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetXunitTheoryMethod(semanticModel).Any();
        }

        public static IEnumerable<MethodDeclarationSyntax> GetXunitFactMethod(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<FactAttribute>(semanticModel);
        }

        public static IEnumerable<MethodDeclarationSyntax> GetXunitTheoryMethod(this ClassDeclarationSyntax @class, SemanticModel semanticModel)
        {
            return @class.GetClassMethods<TheoryAttribute>(semanticModel);
        }
    }
}