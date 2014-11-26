using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.TearDownAttribute;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using System;
using System.Linq;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TearDown : ConverterSpecification<TearDownAttributeConverter>
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
                        [TearDown]
                        public void TestFixtureTearDown()
                        {
                            var i = 0;
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

    public class when_converting_TearDown : behaves_like_converting_TearDown
    {
        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();
            Assert.Equal(code,
                @"using NUnit.Framework;

namespace n2x
{
    public class Test : IDisposable
    {
        [Test]
        public void should_do_the_magic()
        {
        }

        public virtual void Dispose()
        {
            var i = 0;
        }
    }
}");
        }

        [Fact]
        public void should_implement_IDisposable_interface()
        {
            var isImplementedIDisposable = TestClassSyntax.BaseList.Types.OfType<IdentifierNameSyntax>().Any(p => p.Identifier.Text == "IDisposable");

            Assert.True(isImplementedIDisposable);
        }

        [Fact]
        public void should_set_Dispose_method_as_virtual()
        {
            var disposeMethod = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.Identifier.Text == "Dispose");
            var disposeIsVirtual = disposeMethod.Modifiers.Any(p => p.Text == "virtual");

            Assert.NotNull(disposeMethod);
            Assert.True(disposeIsVirtual);
        }

        [Fact]
        public void should_remove_methods_with_TearDown_attributes()
        {
            Assert.False(TestClassSyntax.HasTearDownMethods(SemanticModel));
        }
    }

    public class when_converting_TearDown_at_already_disposable_class : behaves_like_converting_TearDown
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
               @"using NUnit.Framework;

                namespace n2x
                {
                    public class Test : IDisposable
                    {
                        [TearDown]
                        public void TestFixtureTearDown()
                        {
                            var i = 0;
                        }

                        [Test]
                        public void should_do_the_magic()
                        {
                        }

                        public virtual void Dispose()
                        {
                            var j = 0;
                        }
                     }
                }");
        }

        [Fact]
        public void should_not_implement_IDisposable_twice()
        {
            var interfaceCount = TestClassSyntax.BaseList.Types.OfType<IdentifierNameSyntax>().Count(p => p.Identifier.Text == "IDisposable");
            var memberCount = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().Count(m => m.Identifier.Text == "Dispose");

            Assert.Equal(1, interfaceCount);
            Assert.Equal(1, memberCount);
        }
    }

    public class when_converting_TearDown_for_class_with_disposable_base: behaves_like_converting_TearDown
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
               @"using System;
                using NUnit.Framework;

                namespace n2x
                {
                    public class Base : IDisposable
                    {
                        public virtual void Dispose()
                        {
                            var b = 0;
                        }
                    }

                    public class Test : Base
                    {
                        [TearDown]
                        public void TestFixtureTearDown()
                        {
                            var i = 0;
                        }

                        [Test]
                        public void should_do_the_magic()
                        {
                        }
                     }
                }");
        }

        [Fact]
        public void should_set_Dispose_method_as_override()
        {
            var disposeMethod = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.Identifier.Text == "Dispose");
            var disposeIsOverride = disposeMethod.Modifiers.Any(p => p.Text == "override");

            Assert.NotNull(disposeMethod);
            Assert.True(disposeIsOverride);
        }
    }
}
