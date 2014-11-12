using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TestFixtureSetUp_attribute : Specification
    {
        protected TestCode Code;
        private TestFixtureSetUpConverter _converter;
        protected ClassDeclarationSyntax Result;

        public override void Context()
        {
            Code = new TestCode(
               @"using NUnit.Framework;

                public class Test
                {
                    [TestFixtureSetUp]
                    public void TestFixtureSetUp()
                    {
                    }
                }");

            _converter = new TestFixtureSetUpConverter(Code.SemanticModel);
        }

        public override void Because()
        {
            Result = (ClassDeclarationSyntax)_converter.Convert(Code.ClassDeclaration);
        }
    }

    public class WhenConvertingConvertingTestFixtureSetUp : behaves_like_converting_TestFixtureSetUp_attribute
    {
        [Fact]
        public void should_remove_methods_that_are_marked_with_TestFixtureSetUp_attribute()
        {
            var methods = Result.Members.OfType<MethodDeclarationSyntax>();
            var hasTestFixtureSetupMethods = methods.Any(method => method
                .AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(a => Code.SemanticModel.GetTypeInfo(a).Type.IsTestFixtureSetUpAttribute()));

            Assert.False(hasTestFixtureSetupMethods);
        }
    }
}
