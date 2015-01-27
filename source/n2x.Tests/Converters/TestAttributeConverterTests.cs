using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.TestAttribute;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using NUnit.Framework;
using Xunit;
using Assert = Xunit.Assert;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TestAttribute : ConverterSpecification<TestAttributeConverterProvider>
    {
        protected NamespaceDeclarationSyntax NamespaceSyntax { get; set; }
        protected ClassDeclarationSyntax TestClassSyntax { get; set; }
        protected SemanticModel SemanticModel { get; set; }

        public override void Context()
        {
            base.Context();

            Code = new TestCode(
               @"using NUnit.Framework;

                namespace n2x
                {
                    public class Test
                    {
                        [Test]
                        public void should_do_the_magic1()
                        {
                        }

                        [Test]
                        public void should_do_the_magic2()
                        {
                        }
                     }
                }");
        }

        public override void Because()
        {
            base.Because();

            NamespaceSyntax = (NamespaceDeclarationSyntax)Compilation.Members.Single();
            TestClassSyntax =
                NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            SemanticModel = Result.GetSemanticModelAsync().Result;

            Console.Out.WriteLine("{0}", Compilation.ToFullString());
        }
    }

    public class when_converting_TestAttribute : behaves_like_converting_TestAttribute
    {
        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Assert.Equal(code,
                @"using NUnit.Framework;

namespace n2x
{
    public class Test
    {
        [Xunit.FactAttribute]
        public void should_do_the_magic1()
        {
        }

        [Xunit.FactAttribute]
        public void should_do_the_magic2()
        {
        }
    }
}");
        }

        [Fact]
        public void should_remove_TestAttribute()
        {
            var hasTestAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Any(a => a.IsOfType<TestAttribute>(SemanticModel));

            Assert.False(hasTestAttribute);
        }

        [Fact]
        public void should_add_FactAttribute()
        {
            var factAttributeCount = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Count(a => a.IsOfType<FactAttribute>(SemanticModel));

            Assert.Equal(2, factAttributeCount);
        }
    }
}
