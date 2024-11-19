using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;

public class CommandLineOptions
{
    [Option('i', "in-path", Required = false, HelpText = @"Input path of JSON file")]
    public string InputPath { get; set; } = string.Empty;

    [Option('o', "output-path", Required = true, HelpText = "Output path of C Sharp Class files")]
    public string OutputPath { get; set; } = string.Empty;

    [Option('j', "json", Required = false, HelpText = "JSON string")]
    public string JsonText { get; set; } = string.Empty;

    [Option('u', "url", Required = false, HelpText = "URL with JSON response")]
    public string Url { get; set; } = string.Empty;
}