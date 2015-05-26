using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters.ECBAutotests
{
    public class LoggerInjector : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();
            var objectCreations = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>()
                .Where(s => s.Type.IsAncestorOf("BaseExecutableStep", semanticModel)
                            || s.Type.IsAncestorOf("Composition", semanticModel))
                .ToList();
            foreach (var objectCreation in objectCreations)
            {
                var initializer = objectCreation.Initializer ?? SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression);
                var newObjectCreation = objectCreation
                    .WithInitializer(
                        initializer
                        .AddExpressions(
                            SyntaxFactory.ParseExpression(
                                @"Logger = new SeleniumProject.Pipeline.Utils.Logger(_outputHelper)")
                        ))
                    ;
                dict.Add(objectCreation, newObjectCreation);
            }

            return root.ReplaceNodes(dict, normalize: false);
        }
    }
}