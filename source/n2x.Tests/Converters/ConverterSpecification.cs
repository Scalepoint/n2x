using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Converters;
using n2x.Tests.Utils;
using Xunit;

namespace n2x.Tests.Converters
{
    public abstract class ConverterSpecification<TConverterProvider> : Specification
        where TConverterProvider : IConverterProvider, new()
    {
        protected TestCode Code;

        protected DocumentConverter Converter { get; set; }

        protected Document Result;
        protected CompilationUnitSyntax Compilation { get; set; }

        public override void Context()
        {
            base.Context();

            Converter = new DocumentConverter(new TConverterProvider());
        }

        public override void Because()
        {
            Result = Converter.Convert(Code.Document);

            Compilation = (CompilationUnitSyntax) Result.GetSyntaxRootAsync().Result;
        }

        [Fact]
        public void should_not_produce_compilation_errors_and_warnings()
        {
            var hasCompilationErrorsOrWarnings = Compilation.GetDiagnostics()
                .Any(d => d.Severity == DiagnosticSeverity.Error
                          || d.Severity == DiagnosticSeverity.Warning);

            Assert.False(hasCompilationErrorsOrWarnings);
        }
    }
}