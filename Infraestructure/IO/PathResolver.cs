namespace Infrastructure.IO
{
    public static class PathResolver
    {
        public static string ResolveExistingPath(string configuredPath)
        {
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                return configuredPath;
            }

            if (Path.IsPathRooted(configuredPath))
            {
                return configuredPath;
            }

            var current = Directory.GetCurrentDirectory();
            foreach (var candidateRoot in EnumerateCurrentAndParents(current))
            {
                var candidate = Path.Combine(candidateRoot, configuredPath);
                if (File.Exists(candidate) || Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            return Path.Combine(current, configuredPath);
        }

        public static string ResolveDirectory(string configuredPath)
        {
            var resolved = ResolveExistingPath(configuredPath);
            if (!Path.IsPathRooted(resolved))
            {
                resolved = Path.GetFullPath(resolved);
            }

            Directory.CreateDirectory(resolved);
            return resolved;
        }

        private static IEnumerable<string> EnumerateCurrentAndParents(string startPath)
        {
            var current = new DirectoryInfo(startPath);
            while (current is not null)
            {
                yield return current.FullName;
                current = current.Parent;
            }
        }
    }
}
