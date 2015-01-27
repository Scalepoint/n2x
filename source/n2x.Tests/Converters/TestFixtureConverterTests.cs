using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.TestFixture;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using NUnit.Framework;
using Xunit;
using Assert = Xunit.Assert;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TestFixture : ConverterSpecification<TestFixtureConverterProvider>
    {
        protected NamespaceDeclarationSyntax NamespaceSyntax { get; set; }
        protected ClassDeclarationSyntax TestClassSyntax { get; set; }
        protected SemanticModel SemanticModel { get; set; }
        protected ClassDeclarationSyntax TestDataClassSyntax { get; set; }

        public override void Context()
        {
            base.Context();

            Code = new TestCode(
@"using NUnit.Framework;

namespace n2x
{
    public class Test
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var i = 10;
        }

        [TestFixtureTearDown]
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

        public override void Because()
        {
            base.Because();

            NamespaceSyntax = (NamespaceDeclarationSyntax)Compilation.Members.Single();
            TestClassSyntax = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            TestDataClassSyntax = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().SingleOrDefault(c => c.Identifier.Text == "TestData");
            SemanticModel = Result.GetSemanticModelAsync().Result;

            Console.Out.WriteLine("{0}", Compilation.ToFullString());
        }
       
    }

    public class when_converting_TestFixtureSetUpAttribute : behaves_like_converting_TestFixture
    {
        [Fact]
        public void should_remove_TestFixtureSetUp_method_from_test_class()
        {
            var methods = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>();
            var hasTestFixtureSetupMethods = methods.Any(method => method
                .AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(a => a.IsOfType<TestFixtureSetUpAttribute>(SemanticModel)));

            Assert.False(hasTestFixtureSetupMethods);
        }

        [Fact]
        public void should_create_test_data_class()
        {
            Assert.NotNull(TestDataClassSyntax);
        }

        [Fact]
        public void should_add_IUseFixture_implementation_to_test_class()
        {
            Assert.NotNull(TestClassSyntax.BaseList);
            var hasIUseFixtureInterface = TestClassSyntax.BaseList.Types.Any(t => SemanticModel.GetTypeInfo(t).Type.IsIUseFixtureInterfaceOf("n2x.TestData"));

            Assert.True(hasIUseFixtureInterface);
        }

        [Fact]
        public void should_add_SetFixture_method_implementation()
        {
            var setFixtureMethod = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>()
                .SingleOrDefault(m => m.Identifier.Text == "SetFixture");

            Assert.NotNull(setFixtureMethod);
            Assert.Equal(setFixtureMethod.ParameterList.Parameters.Count, 1);
            Assert.Equal(setFixtureMethod.ParameterList.Parameters.First().ToString(), "TestData data");
        }

        [Fact]
        public void should_add_TestData_property()
        {
            var testDataProperty = TestClassSyntax.Members.OfType<PropertyDeclarationSyntax>()
                .SingleOrDefault(p => p.Identifier.Text == "TestData");

            Assert.NotNull(testDataProperty);
        }

        [Fact]
        public void should_assign_TestData_property_in_SetFixture_method()
        {
            var setFixtureMethod = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>()
                .Single(m => m.Identifier.Text == "SetFixture");

            Assert.Equal(setFixtureMethod.Body.ToString(),
                @"{
            TestData = data;
        }");
        }

        [Fact]
        public void should_move_TestFixtureSetUp_method_to_test_data_class_ctor()
        {
            var ctor = TestDataClassSyntax.Members.OfType<ConstructorDeclarationSyntax>().SingleOrDefault();

            Assert.NotNull(ctor);
            Assert.Equal(ctor.Body.ToString(),
                @"{
            var i = 10;
        }");
        }

        [Fact]
        public void should_add_IDisposable_implementation_to_test_class()
        {
            Assert.NotNull(TestDataClassSyntax.BaseList);
            var hasIDisposableInterface = TestDataClassSyntax.BaseList.Types.Any(t => SemanticModel.GetTypeInfo(t).Type.Name == "IDisposable");

            Assert.True(hasIDisposableInterface);

        }

        [Fact]
        public void should_move_TestFixtureTearDown_method_to_test_class_Dispose()
        {
            var dispose = TestDataClassSyntax.Members.OfType<MethodDeclarationSyntax>().SingleOrDefault(m => m.Identifier.Text == "Dispose");
            Assert.NotNull(dispose);

            Assert.Equal(dispose.Body.ToString(), 
                @"{
            var i = 0;
        }");
        }

        [Fact]
        public void should_remove_TestFixtureTearDown_method_from_original_class()
        {
            var methods = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>();
            var hasTestFixtureTearDownMethods = methods.Any(method => method
                .AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(a => a.IsOfType<TestFixtureTearDownAttribute>(SemanticModel)));

            Assert.False(hasTestFixtureTearDownMethods);
        }

        [Fact]
        public void should_remove_TestFixtureSetUp_attribute_from_test_data_class_ctor()
        {
            var ctor = TestDataClassSyntax.Members.OfType<ConstructorDeclarationSyntax>().Single();

            var hasTestFixtureSetupAttribute = ctor
                .AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(a => a.IsOfType<TestFixtureSetUpAttribute>(SemanticModel));

            Assert.False(hasTestFixtureSetupAttribute);
        }

        [Fact]
        public void should_remove_TestFixtureTearDown_attribute_from_Disapose_method()
        {
            var dispose = TestDataClassSyntax.Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Dispose");

            var hasTestFixtureSetupAttribute = dispose
                .AttributeLists
                .SelectMany(a => a.Attributes)
                .Any(a => a.IsOfType<TestFixtureTearDownAttribute>(SemanticModel));

            Assert.False(hasTestFixtureSetupAttribute);
        }

        [Fact]
        public void should_match_etalon_document()
        {
            Assert.Equal(Compilation.ToFullString(),
@"using NUnit.Framework;

namespace n2x
{
    public class TestData : IDisposable
    {
        public TestData()
        {
            var i = 10;
        }

        public void Dispose()
        {
            var i = 0;
        }
    }

    public class Test : Xunit.IUseFixture<TestData>
    {
        [Test]
        public void should_do_the_magic()
        {
        }

        public TestData TestData
        {
            get;
            set;
        }

        public void SetFixture(TestData data)
        {
            TestData = data;
        }
    }
}");
        }

        [Fact]
        public void should_not_produce_compilation_errors_and_warnings()
        {
            var hasCompilationErrorsOrWarnings = Compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning);

            Assert.False(hasCompilationErrorsOrWarnings);
        }
    }
}
