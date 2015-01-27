using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.Using;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_using : ConverterSpecification<UsingConverterProvider>
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
                    public void should_do_the_magic()
                    {
                        var i = 10;
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

    public class when_converting_using : behaves_like_converting_using
    {
        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Assert.Equal(code,
                @"using Xunit;

namespace n2x
{
    public class Test
    {
        public void should_do_the_magic()
        {
            var i = 10;
        }
    }
}");
        }

        [Fact]
        public void should_remove_nunit_using()
        {
            Assert.False(TestClassSyntax.Usings().Any(p => p.Name.ToString() == "NUnit.Framework"));
        }
    }
}