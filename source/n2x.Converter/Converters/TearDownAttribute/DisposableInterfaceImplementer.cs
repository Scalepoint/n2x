using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace n2x.Converter.Converters.TearDownAttribute
{
    internal class DisposableInterfaceImplementer : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var @class in root.Classes())
            {
                var hasTearDownMethod = @class.HasTearDownMethods(semanticModel);

                if (hasTearDownMethod)
                {
                    if (!@class.IsDisposable())
                    {
                        var modifiedClass = @class.AddBaseListTypes(SyntaxFactory.ParseTypeName("System.IDisposable"));

                        dict.Add(@class, modifiedClass);
                    }
                }
            }

            return root.ReplaceNodes(dict);
        }
    }
}