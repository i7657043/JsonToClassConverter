using CommandLine;

public class CommandLineOptions
{
    [Option('i', "--in-path", Required = false, HelpText = @"Input path of JSON file")]
    public string InputPath { get; set; } = string.Empty;

    [Option('o', "--output-path", Required = false, HelpText = "Output path of C Sharp Class files")]
    public string OutputPath { get; set; } = string.Empty;
}