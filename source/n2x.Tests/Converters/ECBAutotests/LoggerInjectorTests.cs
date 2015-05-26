using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.ECBAutotests;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters.ECBAutotests
{
    public abstract class behaves_like_injecting_logger : ConverterSpecification<LoggerInjectorProvider>
    {
        protected NamespaceDeclarationSyntax NamespaceSyntax { get; set; }
        protected ClassDeclarationSyntax TestClassSyntax { get; set; }
        protected SemanticModel SemanticModel { get; set; }

        public override void Because()
        {
            base.Because();

            NamespaceSyntax = (NamespaceDeclarationSyntax)Compilation.Members.Single();
            TestClassSyntax = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            SemanticModel = Result.GetSemanticModelAsync().Result;

            Console.WriteLine("{0}", Compilation.ToFullString());
        }
    }

    public class when_injecting_logger_to_BaseExecutableStep_derived_class_ctor : behaves_like_injecting_logger
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin()
            {
                Driver = null
            };
        }
        }

    public class FastLogin : BaseExecutableStep
    {
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin()
            {
                Driver = null
,Logger = new SeleniumProject.Pipeline.Utils.Logger(_outputHelper)            };
        }
        }

    public class FastLogin : BaseExecutableStep
    {
    }
}");
        }
    }


    public class when_injecting_logger_to_BaseExecutableStep_ancestor_class_ctor : behaves_like_injecting_logger
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin()
            {
                Driver = null
            };
        }
        }

    public class FastLogin : Login
    {
    }

    public class Login : BaseExecutableStep
    {
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin()
            {
                Driver = null
,Logger = new SeleniumProject.Pipeline.Utils.Logger(_outputHelper)            };
        }
        }

    public class FastLogin : Login
    {
    }

    public class Login : BaseExecutableStep
    {
    }
}");
        }
    }

    public class when_injecting_logger_to_Composition_derived_class_ctor : behaves_like_injecting_logger
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin()
            {
                Driver = null
            };
        }
        }

    public class FastLogin : Composition
    {
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin()
            {
                Driver = null
,Logger = new SeleniumProject.Pipeline.Utils.Logger(_outputHelper)            };
        }
        }

    public class FastLogin : Composition
    {
    }
}");
        }
    }

    public class when_injecting_logger_to_Composition_derived_class_ctor_without_initializer : behaves_like_injecting_logger
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin();
        }
        }

    public class FastLogin : Composition
    {
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
        [Test]
        public void should_do_the_magic1()
        {
            new FastLogin(){Logger = new SeleniumProject.Pipeline.Utils.Logger(_outputHelper)};
        }
        }

    public class FastLogin : Composition
    {
    }
}");
        }
    }

}