using System.CommandLine;
using xyz;

var rootCommand = CommandFactory.Build();

ParseResult parseResult = rootCommand.Parse(args);

if (parseResult.Errors.Count == 0)
{
    return parseResult.Invoke();
}

foreach (var error in parseResult.Errors)
{
    Console.WriteLine(error.Message);
}

return 1;
