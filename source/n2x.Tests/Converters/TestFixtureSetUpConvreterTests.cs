using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters;
using n2x.Converter.Converters.TestFixtureSetUp;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TestFixtureSetUp_attribute : Specification
    {
        protected TestCode Code;
        protected TestFixtureSetUpConverter _converter;

        protected Document Result;
        protected CompilationUnitSyntax Compilation { get; set; }
        protected NamespaceDeclarationSyntax NamespaceSyntax { get; set; }
        protected ClassDeclarationSyntax TestClassSyntax { get; set; }
        protected SemanticModel SemanticModel { get; set; }
        protected ClassDeclarationSyntax TestDataClass { get; set; }

        public override void Context()
        {
            Code = new TestCode(
               @"using NUnit.Framework;

                namespace n2x
                {
                    public class Test
                    {
                        [TestFixtureSetUp]
                        public void TestFixtureSetUp()
                        {
                            var i = 10;
                        }

                        [Test]
                        public void should_do_the_magic()
                        {
                        }
                    }
                }");

            _converter = new TestFixtureSetUpConverter();
        }

        public override void Because()
        {
            Result = _converter.Convert(Code.Document);
            Compilation = (CompilationUnitSyntax)Result.GetSyntaxRootAsync().Result;
            NamespaceSyntax = (NamespaceDeclarationSyntax)Compilation.Members.Single();
            TestClassSyntax = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            TestDataClass = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().SingleOrDefault(c => c.Identifier.Text == "TestData");
            SemanticModel = Result.GetSemanticModelAsync().Result;
        }
       
    }

    public class WhenConvertingConvertingTestFixtureSetUp : behaves_like_converting_TestFixtureSetUp_attribute
    {
        [Fact]
        public void should_remove_methods_that_are_marked_with_TestFixtureSetUp_attribute_from_original_class()
        {
            var methods = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>();
            var hasTestFixtureSetupMethods = methods.Any(method => method
                .AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(a => SemanticModel.GetTypeInfo(a).Type.IsTestFixtureSetUpAttribute()));

            Assert.False(hasTestFixtureSetupMethods);
        }

        [Fact]
        public void should_create_test_data_class()
        {
            Assert.NotNull(TestDataClass);
        }
    }
}
