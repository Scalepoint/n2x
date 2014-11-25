using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.SetUp;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using NUnit.Framework;
using Xunit;
using Xunit.Extensions;
using Assert = Xunit.Assert;

namespace n2x.Tests.Converters
{
    public abstract class behaves_like_converting_SetUp : ConverterSpecification<SetUpConverter>
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
                        [SetUp]
                        public void SetUp()
                        {
                            var i = 10;
                        }

                        [Test]
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

    public class when_converting_SetUp : behaves_like_converting_SetUp
    {
        [Fact]
        public void should_add_default_constructor_with_SetUp_logic()
        {
            var ctor = TestClassSyntax.Members.OfType<ConstructorDeclarationSyntax>().SingleOrDefault();

            Assert.NotNull(ctor);
            Assert.Equal(ctor.Body.ToString(),
                @"{
            var i = 10;
        }");
        }

        [Fact]
        public void should_remove_SetUp_method()
        {
            var setUpMethods =
                TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().DecoratedWith<SetUpAttribute>(SemanticModel);
            Assert.Empty(setUpMethods);
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
        [Test]
        public void should_do_the_magic()
        {
        }

        public Test()
        {
            var i = 10;
        }
    }
}");
        }
    }

    public class when_converting_SetUp_for_class_with_default_constructor : behaves_like_converting_SetUp
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
                        public Test()
                        {
                            var n = 500;
                        }

                        [SetUp]
                        public void SetUp()
                        {
                            var i = 10;
                        }

                        [Test]
                        public void should_do_the_magic()
                        {
                        }
                     }
                }");
        }

        [Fact]
        public void should_append_SetUp_method_body_to_default_constructor()
        {
            var ctor = TestClassSyntax.Members.OfType<ConstructorDeclarationSyntax>().SingleOrDefault();

            Assert.NotNull(ctor);
            Assert.Equal(ctor.Body.ToString(),
                @"{
            var n = 500;
            var i = 10;
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
        public Test()
        {
            var n = 500;
            var i = 10;
        }

        [Test]
        public void should_do_the_magic()
        {
        }
    }
}");
        }
    }
}