using System.CommandLine;
using System.Text.RegularExpressions;
using xyz.Search;

namespace xyz;

internal static class CommandFactory
{
    internal static RootCommand Build()
    {
        var fileArgument = BuildFileArgument();
        var regexArgument = BuildRegexArgument();

        var rootCommand = new RootCommand("Examine the contents of a zip file.");

        var searchCommand = BuildSearchCommand(fileArgument, regexArgument);
        var countCommand = BuildCountCommand(fileArgument, regexArgument);

        rootCommand.Subcommands.Add(searchCommand);
        rootCommand.Subcommands.Add(countCommand);

        return rootCommand;
    }

    private static Argument<FileInfo> BuildFileArgument()
    {
        var fileArgument = new Argument<FileInfo>("file")
        {
            Description = "The path to the zip file to examine."
        };
        ArgumentValidation.AcceptExistingOnly(fileArgument);

        return fileArgument;
    }

    private static Argument<string> BuildRegexArgument()
    {
        var regexArgument = new Argument<string>("regex")
        {
            Description = "The regular expression to match against."
        };
        regexArgument.Validators.Add(result =>
        {
            var regex = result.GetRequiredValue(regexArgument);
            try
            {
                Regex.IsMatch("", regex);
            }
            catch (ArgumentException)
            {
                result.AddError("Provided regex value is invalid.");
            }
        });

        return regexArgument;
    }

    private static Command BuildSearchCommand(Argument<FileInfo> fileArgument, Argument<string> regexArgument)
    {
        var fullPathOption = new Option<bool>("--full-path", "-f")
        {
            Description = "Display the entire path to the file in result. Otherwise, truncate relative to zip location.",
            DefaultValueFactory = _ => false
        };

        var searchCommand = new Command("search")
        {
            Description = "List the files in the zip directory with names that match the provided regex."
        };
        searchCommand.Arguments.Add(fileArgument);
        searchCommand.Arguments.Add(regexArgument);
        searchCommand.Options.Add(fullPathOption);

        searchCommand.SetAction(parseResult =>
        {
            var options = new SearchOptions
            {
                TargetFile = parseResult.GetRequiredValue(fileArgument),
                RegularExpression = parseResult.GetRequiredValue(regexArgument),
                ReportFullPath = parseResult.GetValue(fullPathOption)
            };

            var service = new SearchService(options);
            var results = service.ExecuteSearch();

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        });

        return searchCommand;
    }

    private static Command BuildCountCommand(Argument<FileInfo> fileArgument, Argument<string> regexArgument)
    {
        var countCommand = new Command("count")
        {
            Description = "Count the files in the zip directory with names that match the provided regex."
        };
        countCommand.Arguments.Add(fileArgument);
        countCommand.Arguments.Add(regexArgument);

        countCommand.SetAction(parseResult =>
        {
                var options = new SearchOptions
                {
                    TargetFile = parseResult.GetRequiredValue(fileArgument),
                    RegularExpression = parseResult.GetRequiredValue(regexArgument)
                };

                var service = new SearchService(options);
                var result = service.ExecuteCount();

                Console.WriteLine(result);
        });

        return countCommand;
    }
}
