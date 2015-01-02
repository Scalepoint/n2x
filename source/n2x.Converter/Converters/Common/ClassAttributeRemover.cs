using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Common
{
    public class ClassAttributeRemover<T> : IConverter 
        where T : Attribute
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var attributes = root
                .Classes()
                .SelectMany(c => c.GetAttributes<T>(semanticModel))
                .ToList();

            if (attributes.Any())
            {
                return root.RemoveNodes(attributes, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
    }
}