namespace xyz.test;

public class CommandFactoryTests
{
    [Fact]
    public void Build_WhenCalled_ReturnsRootCommand()
    {
        var rootCommand = CommandFactory.Build();

        Assert.Empty(rootCommand.Arguments);
        Assert.Equal("Examine the contents of a zip file.", rootCommand.Description);
        Assert.Single(rootCommand.Subcommands, command => command.Name == "search");
        Assert.Single(rootCommand.Subcommands, command => command.Name == "count");
    }

    [Fact]
    public void Build_WhenCalled_ReturnsRootCommandWithSearchCommand()
    {
        var rootCommand = CommandFactory.Build();
        var searchCommand = rootCommand.Subcommands.Single(command => command.Name == "search");

        Assert.Equal("List the files in the zip directory with names that match the provided regex.", searchCommand.Description);
        Assert.Single(searchCommand.Arguments, argument => argument.Name == "file");
        Assert.Single(searchCommand.Arguments, argument => argument.Name == "regex");
        Assert.Single(searchCommand.Options, option => option.Name == "--full-path");
        Assert.NotNull(searchCommand.Action);

        var searchFileArgument = searchCommand.Arguments.Single(argument => argument.Name == "file");
        Assert.Equal("The path to the zip file to examine.", searchFileArgument.Description);
        Assert.Single(searchFileArgument.Validators);

        var searchRegexArgument = searchCommand.Arguments.Single(argument => argument.Name == "regex");
        Assert.Equal("The regular expression to match against.", searchRegexArgument.Description);
        Assert.Single(searchRegexArgument.Validators);

        var searchFullPathOption = searchCommand.Options.Single(option => option.Name == "--full-path");
        Assert.Single(searchFullPathOption.Aliases, alias => alias == "-f");
        Assert.True(searchFullPathOption.HasDefaultValue);
    }

    [Fact]
    public void Build_WhenCalled_ReturnsRootCommandWithCountCommand()
    {
        var rootCommand = CommandFactory.Build();
        var countCommand = rootCommand.Subcommands.Single(command => command.Name == "count");

        Assert.Equal("Count the files in the zip directory with names that match the provided regex.", countCommand.Description);
        Assert.Single(countCommand.Arguments, argument => argument.Name == "file");
        Assert.Single(countCommand.Arguments, argument => argument.Name == "regex");
        Assert.NotNull(countCommand.Action);

        var searchFileArgument = countCommand.Arguments.Single(argument => argument.Name == "file");
        Assert.Equal("The path to the zip file to examine.", searchFileArgument.Description);
        Assert.Single(searchFileArgument.Validators);

        var searchRegexArgument = countCommand.Arguments.Single(argument => argument.Name == "regex");
        Assert.Equal("The regular expression to match against.", searchRegexArgument.Description);
        Assert.Single(searchRegexArgument.Validators);
    }
}
