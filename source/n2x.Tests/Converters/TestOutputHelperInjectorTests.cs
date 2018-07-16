using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.TestOutputHelperInjector;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public abstract class behaves_like_injecting_OutputHelper : ConverterSpecification<TestOutputHelperInjectorProvider>
    {
        protected NamespaceDeclarationSyntax NamespaceSyntax { get; set; }
        protected ClassDeclarationSyntax TestClassSyntax { get; set; }
        protected SemanticModel SemanticModel { get; set; }

        public override void Because()
        {
            base.Because();

            NamespaceSyntax = (NamespaceDeclarationSyntax) Compilation.Members.Single();
            TestClassSyntax =
                NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            SemanticModel = Result.GetSemanticModelAsync().Result;
        }
    }

    public class when_injecting_output_helper_in_class_without_ctor_and_base_type : behaves_like_injecting_OutputHelper
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                public class Test
                {
                    [Xunit.Fact]
                    public void should_do_the_magic()
                    {
                        var i = 10;
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Assert.Equal(
                @"namespace n2x
{
    public class Test
    {
        [Xunit.Fact]
        public void should_do_the_magic()
        {
            var i = 10;
        }

        public Test(Xunit.Abstractions.ITestOutputHelper outputHelper)
        {
        }
    }
}", code);
        }
    }

    public class when_injecting_output_helper_in_class_with_several_ctors_and_without_base_type : behaves_like_injecting_OutputHelper
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                public class Test
                {
                    public Test()
                    {
                    }

                    public Test(int i)
                    {
                    }

                    [Xunit.Fact]
                    public void should_do_the_magic()
                    {
                        var i = 10;
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Assert.Equal(
                @"namespace n2x
{
    public class Test
    {
        public Test(Xunit.Abstractions.ITestOutputHelper outputHelper)
        {
        }

        public Test(int i, Xunit.Abstractions.ITestOutputHelper outputHelper)
        {
        }

        [Xunit.Fact]
        public void should_do_the_magic()
        {
            var i = 10;
        }
    }
}", code);
        }
    }


    public class when_injecting_output_helper_in_class_without_ctor_but_with_base_type_without_ctor : behaves_like_injecting_OutputHelper
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                public class Test : BaseTest
                {
                    [Xunit.Fact]
                    public void should_do_the_magic()
                    {
                        var i = 10;
                    }
                }

                public class BaseTest
                {
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Assert.Equal(
                @"namespace n2x
{
    public class Test : BaseTest
    {
        [Xunit.Fact]
        public void should_do_the_magic()
        {
            var i = 10;
        }

        public Test(Xunit.Abstractions.ITestOutputHelper outputHelper): base(outputHelper)
        {
        }
    }

    public class BaseTest
    {
    }
}", code);
        }
    }
}