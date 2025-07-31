namespace xyz.Search;

internal class SearchOptions
{
    internal FileInfo TargetFile { get; init; } = null!;
    internal string RegularExpression { get; init; } = null!;
    internal bool ReportFullPath { get; init; } = false;
}
