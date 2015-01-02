using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.TestCaseAttribute;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using NUnit.Framework;
using Xunit;
using Xunit.Extensions;
using Assert = Xunit.Assert;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TestCaseAttribute : ConverterSpecification<TestCaseAttributeConverter>
    {
        protected NamespaceDeclarationSyntax NamespaceSyntax { get; set; }
        protected ClassDeclarationSyntax TestClassSyntax { get; set; }
        protected SemanticModel SemanticModel { get; set; }

        public override void Context()
        {
            Code = new TestCode(
               @"using NUnit.Framework;

                namespace n2x
                {
                    public class Test
                    {
                        [TestCase(""val1"", Category = ""slow"")]
                        public void should_do_the_magic()
                        {
                        }
                     }
                }");

            base.Context();
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

    public class when_converting_TestCase : behaves_like_converting_TestCaseAttribute
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
        [Xunit.TheoryAttribute]
        [Xunit.InlineDataAttribute(""val1"")]
        public void should_do_the_magic()
        {
        }
    }
}");
        }

        [Fact]
        public void should_remove_TestCaseAttribute()
        {
            var hasTestAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Any(a => a.IsOfType<TestCaseAttribute>(SemanticModel));

            Assert.False(hasTestAttribute);
        }

        [Fact]
        public void should_add_Theory_attribute()
        {
            var theoryAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .FirstOrDefault(a => a.IsOfType<Xunit.TheoryAttribute>(SemanticModel));

            Assert.NotNull(theoryAttribute);
        }

        [Fact]
        public void should_add_InlineData_attribute()
        {
            var inlineDataAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .FirstOrDefault(a => a.IsOfType<InlineDataAttribute>(SemanticModel));

            Assert.NotNull(inlineDataAttribute);
        }
    }

    public class when_converting_TestCase_with_multiple_cases : behaves_like_converting_TestCaseAttribute
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
               @"using NUnit.Framework;

                namespace n2x
                {
                    public class Test
                    {
                        [TestCase(""val1"")]
                        [TestCase(""val2"")]
                        public void should_do_the_magic()
                        {
                        }
                     }
                }");
        }

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
        [Xunit.TheoryAttribute]
        [Xunit.InlineDataAttribute(""val1"")]
        [Xunit.InlineDataAttribute(""val2"")]
        public void should_do_the_magic()
        {
        }
    }
}");
        }

        [Fact]
        public void should_add_only_one_Theory_attribute()
        {
            var theoryAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .FirstOrDefault(a => a.IsOfType<Xunit.TheoryAttribute>(SemanticModel));

            Assert.NotNull(theoryAttribute);
        }

        [Fact]
        public void should_add_InlineData_attributes_for_each_case()
        {
            var inlineDataAttributes = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Count(a => a.IsOfType<InlineDataAttribute>(SemanticModel));

            Assert.Equal(2, inlineDataAttributes);
        }
    }
}