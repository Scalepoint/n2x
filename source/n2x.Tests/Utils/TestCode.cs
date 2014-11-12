using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace n2x.Tests.Utils
{
    public class TestCode
    {
        //public int Position { get; private set; }
        //public string Text { get; private set; }

        public SyntaxTree SyntaxTree { get; private set; }

        public SyntaxNode SyntaxTreeRoot => SyntaxTree.GetRoot();

        //public SyntaxToken Token { get; private set; }
        //public SyntaxNode SyntaxNode { get; private set; }

        public Compilation Compilation { get; private set; }
        public SemanticModel SemanticModel { get; private set; }

        public ClassDeclarationSyntax ClassDeclaration
        {
            get
            {
                var compil = (CompilationUnitSyntax)SyntaxTreeRoot;
                return (ClassDeclarationSyntax) compil.Members.Single();
            }
        }

        public TestCode(string text)
        {
            SyntaxTree = SyntaxFactory.ParseSyntaxTree(text);

            var systemAsseemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var nUnitAssemblyPath = Path.GetDirectoryName(typeof(TestFixtureAttribute).Assembly.Location);
            Compilation = CSharpCompilation
                .Create("test")
                .AddReferences(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "mscorlib.dll")))
                .AddReferences(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "System.dll")))
                .AddReferences(new MetadataFileReference(Path.Combine(systemAsseemblyPath, "System.Core.dll")))
                .AddReferences(new MetadataFileReference(Path.Combine(nUnitAssemblyPath, "nunit.framework.dll")))
                .AddSyntaxTrees(SyntaxTree);


            SemanticModel = Compilation.GetSemanticModel(SyntaxTree);
        }
    }
}