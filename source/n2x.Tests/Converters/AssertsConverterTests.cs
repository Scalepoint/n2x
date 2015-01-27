using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.Asserts;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_Asserts : ConverterSpecification<AssertsConverterProvider>
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
                    public void should_do_the_magic()
                    {
                        var i = 10;
                        Assert.AreEqual(""a"", ""a"");
                        Assert.AreNotEqual(""a"", ""b"");
                        Assert.IsTrue(true);
                        Assert.IsFalse(false);
                        Assert.IsNull(null);
                    }
                }
            }");
        }

        public override void Because()
        {
            base.Because();

            NamespaceSyntax = (NamespaceDeclarationSyntax)Compilation.Members.Single();
            TestClassSyntax = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            SemanticModel = Result.GetSemanticModelAsync().Result;

            Console.Out.WriteLine("{0}", Compilation.ToFullString());
        }
    }

    public class when_converting_simple_asserts : behaves_like_converting_Asserts
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
        public void should_do_the_magic()
        {
            var i = 10;
            Xunit.Assert.Equal(""a"", ""a"");
            Xunit.Assert.NotEqual(""a"", ""b"");
            Xunit.Assert.True(true);
            Xunit.Assert.False(false);
            Xunit.Assert.Null(null);
        }
    }
}");
        }

        [Fact]
        public void should_remove_all_nunit_asserts()
        {
            var method = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().First();
            Assert.False(method.Body.Statements.Any(p => p.ToString().StartsWith("Assert.")));
        }

        [Fact]
        public void should_add_xunit_asserts()
        {
            var method = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().First();
            Assert.Equal(5, method.Body.Statements.Count(p => p.ToString().StartsWith("Xunit.Assert.")));
        }
    }

    public class when_converting_That_asserts : behaves_like_converting_Asserts
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
                    public void should_do_the_magic()
                    {
                        var i = 10;
                        Assert.That(i, Is.EqualTo(10));
                        Assert.That(i, Is.Not.EqualTo(20));
                        Assert.That(true);
                        Assert.That(null, Is.Null);
                        Assert.That(""ololo"", Is.StringContaining(""ol""));
                        Assert.That(true, ""comment"");
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
        public void should_do_the_magic()
        {
            var i = 10;
            Xunit.Assert.Equal(i, 10);
            Xunit.Assert.NotEqual(i, 20);
            Xunit.Assert.True(true);
            Xunit.Assert.Null(null);
            Xunit.Assert.Contains(""ol"", ""ololo"");
            Xunit.Assert.True(true);
        }
    }
}");
        }

        [Fact]
        public void should_remove_all_nunit_asserts()
        {
            var method = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().First();
            Assert.False(method.Body.Statements.Any(p => p.ToString().StartsWith("Assert.")));
        }

        [Fact]
        public void should_add_xunit_asserts()
        {
            var method = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().First();
            Assert.Equal(6, method.Body.Statements.Count(p => p.ToString().StartsWith("Xunit.Assert.")));
        }
    }
}