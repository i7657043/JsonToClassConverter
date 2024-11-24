using CommandLine;

public class CommandLineOptions
{
    [Option('f', "file", Required = false, HelpText = @"Input path of JSON file")]
    public string FilePath { get; set; } = string.Empty;

    [Option('o', "out", Required = true, HelpText = "Output path of C Sharp Class files")]
    public string OutputPath { get; set; } = string.Empty;

    [Option('j', "json", Required = false, HelpText = "JSON string")]
    public string JsonText { get; set; } = string.Empty;

    [Option('u', "url", Required = false, HelpText = "URL with JSON response")]
    public string Url { get; set; } = string.Empty;
}