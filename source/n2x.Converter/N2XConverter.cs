﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis.MSBuild;

namespace n2x.Converter
{
    public class N2XConverter : IN2XConverter
    {
        private readonly IDocumentConverterProvider _documentConverterProvider;

        public N2XConverter(IDocumentConverterProvider documentConverterProvider)
        {
            _documentConverterProvider = documentConverterProvider;
        }

        public bool ConvertSolution(string solutionFilePath, Action<int, int> progress = null)
        {
            var workspace = MSBuildWorkspace.Create();

            var originalSolution = workspace.OpenSolutionAsync(solutionFilePath).Result;
            var newSolution = originalSolution;
            var converters = _documentConverterProvider.Converters;
            var total = originalSolution.ProjectIds
                .Select(id => originalSolution
                    .GetProject(id))
                .SelectMany(p => p.Documents)
                .Count();

            var processed = 0;

            foreach (var projectId in originalSolution.ProjectIds)
            {
                var project = newSolution.GetProject(projectId);

                foreach (var documentId in project.DocumentIds)
                {
                    var document = newSolution.GetDocument(documentId);
                    var newDocument = document;

                    foreach (var converter in converters)
                    {
                        newDocument = converter.Convert(newDocument);
                    }
                    
                    progress?.Invoke(total, ++processed);
                    newSolution = newDocument.Project.Solution;
                }
            }

            return workspace.TryApplyChanges(newSolution);
        }
    }
}