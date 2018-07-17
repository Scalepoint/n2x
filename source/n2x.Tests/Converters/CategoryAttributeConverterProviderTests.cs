using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.CategoryAttribute;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_CategoryAttribute : ConverterSpecification<CategoryAttributeConverterProvider>
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
                    [TestFixture]
                    [Category(TestCategoryProvider.Smoke)]
                    [Category(TestCategoryProvider.FullRegression)]
                    public class Test
                    {
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

    public class when_converting_Category_Attribute_on_class : behaves_like_converting_CategoryAttribute
    {
        [Fact]
        public void should_remove_CategoryAttribute()
        {
            var hasCategoryAttribute = TestClassSyntax.AttributeLists.SelectMany(a => a.Attributes)
                .Any(a => a.IsOfType<NUnit.Framework.CategoryAttribute>(SemanticModel));

            Assert.False(hasCategoryAttribute);
        }

        [Fact]
        public void should_add_TraitAttribute()
        {
            var traitAttribute = TestClassSyntax.AttributeLists.SelectMany(a => a.Attributes)
                .FirstOrDefault(a => a.IsOfType<Xunit.TraitAttribute>(SemanticModel));

            Assert.NotNull(traitAttribute);

            var categoryArgument = traitAttribute.ArgumentList?.Arguments.First();
            Assert.NotNull(categoryArgument);

            Assert.Equal("\"Category\"", categoryArgument.ToString());

            var valueArgument = traitAttribute.ArgumentList?.Arguments.Skip(1).First();
            Assert.NotNull(valueArgument);

            Assert.Equal("TestCategoryProvider.Smoke", valueArgument.ToString());
        }

        [Fact]
        public void should_replace_several_Category_attributes()
        {
            var traitAttributeCount = TestClassSyntax.AttributeLists.SelectMany(a => a.Attributes)
                .Count(a => a.IsOfType<Xunit.TraitAttribute>(SemanticModel));

            Assert.Equal(2, traitAttributeCount);
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Assert.Equal(
                @"using NUnit.Framework;

namespace n2x
{
    [TestFixture]
    [Xunit.TraitAttribute(""Category"", TestCategoryProvider.Smoke)]
    [Xunit.TraitAttribute(""Category"", TestCategoryProvider.FullRegression)]
    public class Test
    {
    }
}", code);
        }
    }
}