using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using n2x.Converter.Utils;

namespace n2x.Converter.Converters
{
    public class TestFixtureSetUpConverter : IClassConverter
    {
        private readonly SemanticModel _semanticModel;

        public TestFixtureSetUpConverter(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
        }

        public SyntaxNode Convert(ClassDeclarationSyntax @class)
        {
            var methods = @class.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.AttributeLists
                    .SelectMany(a => a.Attributes)
                    .Any(a => _semanticModel.GetTypeInfo(a).Type.IsTestFixtureSetUpAttribute()));

            return @class.RemoveNodes(methods, SyntaxRemoveOptions.KeepNoTrivia);
        }
    }
}