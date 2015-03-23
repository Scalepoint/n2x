using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.MoveToPublic;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{

    public abstract class behaves_like_converting_internal_classes : ConverterSpecification<MoveToPublicProvider>
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

            Console.Out.WriteLine("{0}", Compilation.ToFullString());
        }
    }

    public class when_converting_internal_class_with_Fact_attribute : behaves_like_converting_internal_classes
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                internal class Test
                {
                    [Xunit.Fact]
                    public void should_do_the_magic()
                    {
                        Xunit.Assert.True(true);
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();


            Assert.Equal(code,
                @"namespace n2x
{
    public class Test
    {
        [Xunit.Fact]
        public void should_do_the_magic()
        {
            Xunit.Assert.True(true);
        }
    }
}");
        }
    }

    public class when_converting_internal_class_with_Theory_attribute : behaves_like_converting_internal_classes
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                internal class Test
                {
                    [Xunit.Theory]
                    public void should_do_the_magic()
                    {
                        Xunit.Assert.True(true);
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();


            Assert.Equal(code,
                @"namespace n2x
{
    public class Test
    {
        [Xunit.Theory]
        public void should_do_the_magic()
        {
            Xunit.Assert.True(true);
        }
    }
}");
        }

    }

    public class when_converting_class_with_modifier_by_default_and_Theory_attribute : behaves_like_converting_internal_classes
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                class Test
                {
                    [Xunit.Theory]
                    public void should_do_the_magic()
                    {
                        Xunit.Assert.True(true);
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();


            Assert.Equal(code,
                @"namespace n2x
{
    public class Test
    {
        [Xunit.Theory]
        public void should_do_the_magic()
        {
            Xunit.Assert.True(true);
        }
    }
}");
        }

    }

    public class when_converting_sealed_class_with_Theory_attribute : behaves_like_converting_internal_classes
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                sealed class Test
                {
                    [Xunit.Theory]
                    public void should_do_the_magic()
                    {
                        Xunit.Assert.True(true);
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();


            Assert.Equal(code,
                @"namespace n2x
{
    public sealed class Test
    {
        [Xunit.Theory]
        public void should_do_the_magic()
        {
            Xunit.Assert.True(true);
        }
    }
}");
        }

    }

    public class when_converting_internal_class_with_Theory_attribute_and_comments : behaves_like_converting_internal_classes
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                //http://jira.scalepoint.com/browse/ECB-4446
                //Add possibility to send message to pools without email
                internal class Test : System.Object
                {
                    [Xunit.Theory]
                    public void should_do_the_magic()
                    {
                        Xunit.Assert.True(true);
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Console.WriteLine("code = {0}", code);
            Assert.Equal(code,
                @"namespace n2x
{
    //http://jira.scalepoint.com/browse/ECB-4446
    //Add possibility to send message to pools without email
    public class Test : System.Object
    {
        [Xunit.Theory]
        public void should_do_the_magic()
        {
            Xunit.Assert.True(true);
        }
    }
}");
        }
    }


    public class when_converting_class_without_modifiers_with_Theory_attribute_and_comments : behaves_like_converting_internal_classes
    {
        public override void Context()
        {
            base.Context();

            Code = new TestCode(
                @"namespace n2x
            {
                //http://jira.scalepoint.com/browse/ECB-3749
                //Verify: ECB-3553 New email for unregistered XC
                class Test : System.Object
                {
                    [Xunit.Theory]
                    public void should_do_the_magic()
                    {
                        Xunit.Assert.True(true);
                    }
                }
            }");
        }

        [Fact]
        public void should_match_etalon_document()
        {
            var code = Compilation.ToFullString();

            Console.WriteLine("code = {0}", code);
            Assert.Equal(code,
                @"namespace n2x
{
    //http://jira.scalepoint.com/browse/ECB-3749
    //Verify: ECB-3553 New email for unregistered XC
    public class Test : System.Object
    {
        [Xunit.Theory]
        public void should_do_the_magic()
        {
            Xunit.Assert.True(true);
        }
    }
}");
        }
    }
}