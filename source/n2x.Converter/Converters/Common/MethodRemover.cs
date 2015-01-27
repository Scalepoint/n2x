using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Common
{
    public class MethodRemover<T> : IConverter
        where T : Attribute
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var methods = root
                .Classes()
                .SelectMany(c => c.GetClassMethods<T>(semanticModel))
                .ToList();


            return root.RemoveNodes(methods);
        }
    }
}