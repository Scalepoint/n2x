using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Generators;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Using
{
    public class UsingReplacer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var usings = root.Usings().Where(p => p.Name.ToString() == "NUnit.Framework").ToList();

            if (!usings.Any())
            {
                return root;
            }

            return root
                   .ReplaceNodes(usings, (n1, n2) => ExpressionGenerator.GenerateXunitUsing())
                   .NormalizeWhitespace();
        }
    }
}