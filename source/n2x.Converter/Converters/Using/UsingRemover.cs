using System.Linq;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.Using
{
    public class UsingRemover : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var usings = root.Usings().Where(p => p.Name.ToString() == "NUnit.Framework").ToList();

            if (!usings.Any())
            {
                return root;
            }

            return root.RemoveNodes(usings);
        }
    }
}