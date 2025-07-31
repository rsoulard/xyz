using xyz.Search;

namespace xyz.test.Search;

public class SearchServiceTests
{
    [Fact]
    public void ExecuteSearch_ResultIsInZip_ReturnsExpectedResult()
    {
        var testFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExampleZip.zip"));

        Assert.True(testFile.Exists);

        var searchOptions = new SearchOptions
        {
            TargetFile = testFile,
            RegularExpression = @"^IAmA500KBCSV.csv$"
        };

        var searchService = new SearchService(searchOptions);
        var result = searchService.ExecuteSearch();

        Assert.Single(result);
        Assert.Single(result, result => result == @"IAmA500KBCSV.csv");
    }

    [Fact]
    public void ExecuteSearch_MultipleResultsInZip_ReturnsExpectedResults()
    {
        var testFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExampleZip.zip"));

        Assert.True(testFile.Exists);

        var searchOptions = new SearchOptions
        {
            TargetFile = testFile,
            RegularExpression = @"^IAmATextDocument.txt$"
        };

        var searchService = new SearchService(searchOptions);
        var result = searchService.ExecuteSearch();

        Assert.Contains(result, result => result == @"IAmATextDocument.txt");
        Assert.Contains(result, result => result == Path.Combine(@"IAmANestedZip", @"IAmATextDocument.txt"));
    }

    [Fact]
    public void ExecuteSearch_ResultIsInNestedZip_ReturnsExpectedResult()
    {
        var testFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExampleZip.zip"));

        Assert.True(testFile.Exists);

        var searchOptions = new SearchOptions
        {
            TargetFile = testFile,
            RegularExpression = @"^IAmA500KBPDF.pdf$"
        };

        var searchService = new SearchService(searchOptions);
        var result = searchService.ExecuteSearch();

        Assert.Single(result);
        Assert.Single(result, result => result == Path.Combine(@"IAmANestedZip", @"IAmA500KBPDF.pdf"));
    }

    [Fact]
    public void ExecuteSearch_FullPathEnabled_ReturnsFullPathOfExpectedResult()
    {
        var testFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExampleZip.zip"));

        Assert.True(testFile.Exists);

        var searchOptions = new SearchOptions
        {
            TargetFile = testFile,
            RegularExpression = @"^IAmA500KBCSV.csv$",
            ReportFullPath = true
        };

        var searchService = new SearchService(searchOptions);
        var result = searchService.ExecuteSearch();

        Assert.Single(result);
        Assert.Single(result, result => result == Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExampleZip", @"IAmA500KBCSV.csv"));
    }

    [Theory]
    [InlineData(@"^IAmATextDocument\.txt", 2)]
    [InlineData(@"^.{30,}$", 2)]
    [InlineData(@"^.*\.png$", 4)]
    [InlineData(@"^IAmA", 17)]
    public void ExecuteCount_ValidRegex_ReturnsExpectedCounts(string regex, int expectedCount)
    {
        var testFile = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExampleZip.zip"));

        Assert.True(testFile.Exists);

        var searchOptions = new SearchOptions
        {
            TargetFile = testFile,
            RegularExpression = regex
        };

        var searchService = new SearchService(searchOptions);
        var result = searchService.ExecuteCount();

        Assert.Equal(expectedCount, result);
    }
}
