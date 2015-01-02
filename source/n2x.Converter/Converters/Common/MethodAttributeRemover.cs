using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Common
{
    public class MethodAttributeRemover<T> : IConverter
        where T : Attribute
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var attributes = root
                .Classes()
                .SelectMany(c => c.Members.OfType<MethodDeclarationSyntax>())
                .SelectMany(m => m.GetAttributes<T>(semanticModel))
                .ToList();

            if (attributes.Any())
            {
                return root.RemoveNodes(attributes, SyntaxRemoveOptions.KeepNoTrivia);
            }

            return root;
        }
    }
}