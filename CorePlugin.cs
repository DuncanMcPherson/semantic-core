using System;
using SemanticRelease.Abstractions;

namespace SemanticRelease.Core
{
    public class CorePlugin : ISemanticPlugin
    {
        public void Register(SemanticLifecycle lifecycle)
        {
            lifecycle.OnAnalyzeCommits(async context =>
            {
                
            });
        }
    }
}