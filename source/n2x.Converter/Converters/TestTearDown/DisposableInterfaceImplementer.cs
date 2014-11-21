using System;
using Microsoft.CodeAnalysis;
using n2x.Converter.Utils;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace n2x.Converter.Converters.TestTearDown
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
                        var modifiedClass = @class.AddBaseListTypes(SyntaxFactory.ParseTypeName("IDisposable"));

                        dict.Add(@class, modifiedClass);
                    }
                }
            }

            if (dict.Any())
            {
                return root.ReplaceNodes(dict.Keys, (n1, n2) => dict[n1]);
            }

            return root;
        }
    }
}