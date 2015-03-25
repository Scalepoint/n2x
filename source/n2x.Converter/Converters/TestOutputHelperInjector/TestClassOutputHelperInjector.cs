using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Generators;
using n2x.Converter.Utils;
using Xunit.Abstractions;

namespace n2x.Converter.Converters.TestOutputHelperInjector
{
    public class TestClassOutputHelperInjector : IConverter
    {
        public SyntaxNode Convert(SyntaxNode root, SemanticModel semanticModel)
        {
            var dict = new Dictionary<SyntaxNode, SyntaxNode>();

            var testClasses = root.Classes()
                .Where(c => c.IsXUnitTestClass(semanticModel));

            foreach (var testClass in testClasses)
            {
                var outputHelperParameter = SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier("outputHelper"))
                    .WithType(ExpressionGenerator.ParseType<ITestOutputHelper>());

                var outputHelperArgument = SyntaxFactory.Argument(SyntaxFactory.ParseExpression("outputHelper"));

                var hasBaseTestClasses =
                    testClass.BaseList?.Types.Any(t => semanticModel.GetTypeInfo(t.Type).Type?.TypeKind == TypeKind.Class
                                                      && testClass.GetAssemblyName(semanticModel) == t.Type.GetAssemblyName(semanticModel));

                var ctors = testClass.Ctors().ToList();
                if (!ctors.Any())
                {
                    var ctor = ExpressionGenerator.GeneratePublicConstructor(testClass.Identifier.Text, outputHelperParameter);
                    ctor = AddBaseInitializerIfRequired(hasBaseTestClasses, ctor, outputHelperArgument);

                    var modifiedClass = testClass.AddMembers(ctor);
                    dict.Add(testClass, modifiedClass);
                }
                else
                {
                    foreach (var ctor in ctors)
                    {
                        var modifiedCtor = ctor.AddParameterListParameters(outputHelperParameter);
                        modifiedCtor = AddBaseInitializerIfRequired(hasBaseTestClasses, modifiedCtor, outputHelperArgument);

                        dict.Add(ctor, modifiedCtor);
                    }
                }
            }

            return root.ReplaceNodes(dict);
        }

        private static ConstructorDeclarationSyntax AddBaseInitializerIfRequired(bool? hasBaseTestClasses,
            ConstructorDeclarationSyntax ctor,
            ArgumentSyntax outputHelperArgument)
        {
            if (hasBaseTestClasses.HasValue && hasBaseTestClasses.Value)
            {
                return ctor.WithInitializer(
                    SyntaxFactory.ConstructorInitializer(
                        SyntaxKind.BaseConstructorInitializer,
                        SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] {outputHelperArgument}))));
            }

            return ctor;
        }
    }
}