using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace xyz.Search;

internal class SearchService(SearchOptions options)
{
    private readonly Regex searchPattern = new(options.RegularExpression, RegexOptions.Compiled);
    private readonly Stack<string> path = new();

    internal List<string> ExecuteSearch()
    {
        using var zipStream = new FileStream(options.TargetFile.FullName, FileMode.Open, FileAccess.Read);

        if (options.ReportFullPath)
        {
            var fileWithoutExtension = Path.GetFileNameWithoutExtension(options.TargetFile.FullName);
            path.Push(Path.Combine(options.TargetFile.DirectoryName!, fileWithoutExtension));
        }

        return [.. FindMatchingFiles(zipStream)];
    }

    internal int ExecuteCount()
    {
        using var zipStream = new FileStream(options.TargetFile.FullName, FileMode.Open, FileAccess.Read);

        return FindMatchingFiles(zipStream).Count();
    }

    private IEnumerable<string> FindMatchingFiles(Stream zipStream)
    {
        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var entry in zip.Entries)
        {
            var nameValue = entry.Name == "" ? entry.FullName : entry.Name;

            if (searchPattern.IsMatch(nameValue))
            {
                var fullPath = path.Reverse()
                    .Aggregate(new StringBuilder(), (fullPath, part) => fullPath.Append($"{part}{Path.DirectorySeparatorChar}"));

                fullPath.Append(entry.FullName);
                yield return fullPath.ToString();
            }

            if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                path.Push(Path.GetFileNameWithoutExtension(entry.Name));
                using var nestedZipStream = entry.Open();
                foreach (var match in FindMatchingFiles(nestedZipStream))
                {
                    yield return match;
                }
                path.Pop();
            }
        }
    }
}
