using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters.TestAttribute;
using n2x.Converter.Utils;
using n2x.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace n2x.Tests.Converters
{
    public class behaves_like_converting_TestAttribute : Specification
    {
        protected TestCode Code;
        protected TestAttributeConverter _converter;

        protected Document Result;
        protected CompilationUnitSyntax Compilation { get; set; }
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
                        [Test]
                        public void should_do_the_magic1()
                        {
                        }

                        [Test]
                        public void should_do_the_magic2()
                        {
                        }
                     }
                }");

            _converter = new TestAttributeConverter();
        }

        public override void Because()
        {
            Result = _converter.Convert(Code.Document);

            Compilation = (CompilationUnitSyntax)Result.GetSyntaxRootAsync().Result;
            NamespaceSyntax = (NamespaceDeclarationSyntax)Compilation.Members.Single();
            TestClassSyntax = NamespaceSyntax.Members.OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Test");
            SemanticModel = Result.GetSemanticModelAsync().Result;

            Console.Out.WriteLine("{0}", Compilation.ToFullString());
        }
    }

    public class when_converting_TestAttribute : behaves_like_converting_TestAttribute
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
        [Xunit.FactAttribute]
        public void should_do_the_magic1()
        {
        }

        [Xunit.FactAttribute]
        public void should_do_the_magic2()
        {
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

        [Fact]
        public void should_remove_TestAttribute()
        {
            var hasTestAttribute = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Any(a => a.IsOfType<NUnit.Framework.TestAttribute>(SemanticModel));

            Assert.False(hasTestAttribute);
        }

        [Fact]
        public void should_add_FactAttribute()
        {
            var factAttributeCount = TestClassSyntax.Members.OfType<MethodDeclarationSyntax>().SelectMany(p => p.AttributeLists.SelectMany(a => a.Attributes))
                .Count(a => a.IsOfType<FactAttribute>(SemanticModel));

            Assert.Equal(2, factAttributeCount);
        }
    }
}
