using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace n2x.Converter.Converters
{
    public abstract class DocumentConverter : IDocumentConverter
    {
        public virtual Document Convert(Document document)
        {
            var result = document;

            foreach (var converter in GetConverters())
            {
                var root = result.GetSyntaxRootAsync().Result;
                var semanticModel = result.GetSemanticModelAsync().Result;

                var newRoot = converter.Convert(root, semanticModel);
                result = result.WithSyntaxRoot(newRoot);
            }

            return result;
        }

        protected abstract IEnumerable<IConverter> GetConverters();
    }
}