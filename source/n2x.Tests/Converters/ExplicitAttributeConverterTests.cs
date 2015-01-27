using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.ExplicitAttribute;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using NUnit.Framework;
using Xunit;
using Assert = Xunit.Assert;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_ExplicitAttribute : ConverterSpecification<ExplicitAttributeConverterProvider>
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
                        [Explicit]
                        [Xunit.FactAttribute]
                        public void should_do_the_magic()
                        {
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

    public class when_converting_ExplicitAttribute_on_method : behaves_like_converting_ExplicitAttribute
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
        [Xunit.FactAttribute(Skip = ""Explicit"")]
        public void should_do_the_magic()
        {
        }
    }
}");
        }

        [Fact]
        public void should_remove_ExplicitAttribute()
        {
            var hasExplicitAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Any(a => a.IsOfType<ExplicitAttribute>(SemanticModel));

            Assert.False(hasExplicitAttribute);
        }

        [Fact]
        public void should_add_FactAttribute()
        {
            var factAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .FirstOrDefault(a => a.IsOfType<FactAttribute>(SemanticModel));
            Assert.NotNull(factAttribute);

            var skipArgument = factAttribute.ArgumentList?.Arguments.First();
            Assert.NotNull(skipArgument);

            Assert.Equal("Skip = \"Explicit\"", skipArgument.ToString());
        }
    }

    public class when_converting_ExplicitAttribute_on_class : behaves_like_converting_ExplicitAttribute
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
               @"using NUnit.Framework;

                namespace n2x
                {
                    [Explicit]
                    public class Test
                    {
                        [Xunit.FactAttribute]
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
        [Xunit.FactAttribute(Skip = ""Explicit"")]
        public void should_do_the_magic()
        {
        }
    }
}");
        }

        [Fact]
        public void should_remove_ExplicitAttribute()
        {
            var hasExplicitAttribute = TestClassSyntax.AttributeLists.SelectMany(a => a.Attributes)
                .Any(a => a.IsOfType<ExplicitAttribute>(SemanticModel));

            Assert.False(hasExplicitAttribute);
        }

        [Fact]
        public void should_add_FactAttribute()
        {
            var factAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .FirstOrDefault(a => a.IsOfType<FactAttribute>(SemanticModel));
            Assert.NotNull(factAttribute);

            var skipArgument = factAttribute.ArgumentList?.Arguments.First();
            Assert.NotNull(skipArgument);

            Assert.Equal("Skip = \"Explicit\"", skipArgument.ToString());
        }
    }
}
