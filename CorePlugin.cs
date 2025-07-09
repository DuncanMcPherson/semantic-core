using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using SemanticRelease.Abstractions;

namespace SemanticRelease.Core
{
    public class CorePlugin : ISemanticPlugin
    {
        public void Register(SemanticLifecycle lifecycle)
        {
            lifecycle.OnAnalyzeCommits(async context =>
            {
                Console.WriteLine("Collecting commits and previous version information...");
                var repo = new Repository(context.WorkingDirectory);
                var lastTag = GetLastTag(repo, context);
                if (lastTag != null)
                {
                    Console.WriteLine($"Last tag is {lastTag.FriendlyName} at {lastTag.Target.Sha}");
                }
                var commits = await GetCommitsSinceLastTagAsync(repo, lastTag);
                context.PluginData["commits"] = commits;
                context.PluginData["lastTag"] = lastTag != null ? lastTag.FriendlyName : "0.0.0";
                Console.WriteLine($"Found {commits.Count} commits");
            });
        }

        private Tag? GetLastTag(Repository repo, ReleaseContext context)
        {
            var (prefix, suffix) = ExtractTagPattern(context.Config.TagFormat);
            var matchingTags = repo.Tags
                .Where(tag => tag.FriendlyName.StartsWith(prefix) && tag.FriendlyName.EndsWith(suffix))
                .Select(tag => new
                {
                    Tag = tag,
                    Commit = repo.Lookup<Commit>(tag.Target.Id)
                })
                .Where(x => x.Commit != null)
                .OrderByDescending(x => x.Commit.Committer.When)
                .FirstOrDefault();
            return matchingTags?.Tag;
        }

        private (string, string) ExtractTagPattern(string tagFormat)
        {
            const string token = "{version}";
            var index = tagFormat.IndexOf(token, StringComparison.Ordinal);
            if (index < 0)
            {
                throw new ArgumentException($"Tag format {tagFormat} does not contain {token}");
            }
            var prefix = tagFormat[..index];
            var suffix = tagFormat[(index + token.Length)..];
            return (prefix, suffix);
        }

        private async Task<List<Commit>> GetCommitsSinceLastTagAsync(Repository repo,
            Tag? latestTag)
        {
            return await Task.Run(() =>
            {
                Commit? taggedCommit = null;
                if (latestTag != null)
                {
                    taggedCommit = repo.Lookup<Commit>(latestTag.Target.Id);
                }

                var filter = new CommitFilter
                {
                    IncludeReachableFrom = repo.Head,
                    SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time
                };

                return repo.Commits.QueryBy(filter)
                    .TakeWhile(commit => taggedCommit == null || commit.Sha != taggedCommit.Sha).ToList();
            });
        }
    }
}