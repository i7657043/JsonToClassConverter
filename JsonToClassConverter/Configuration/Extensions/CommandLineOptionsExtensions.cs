public static class CommandLineOptionsExtensions
{
    public static void ValidateArgs(this CommandLineOptions commandLineOptions, CommandLineOptions options)
    {
        if (!string.IsNullOrEmpty(options.Url) && !string.IsNullOrEmpty(options.FilePath) && !string.IsNullOrEmpty(options.JsonText))
            throw new ArgumentException("You must not use -u, -f or -j args together");
        else if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.FilePath) && string.IsNullOrEmpty(options.JsonText))
            throw new ArgumentException("You must use only one -u, -f or -j args");
        else if (!string.IsNullOrEmpty(options.Url) && !Uri.TryCreate(options.Url, UriKind.Absolute, out _))
            throw new ArgumentException($"\"{commandLineOptions.Url}\" is not a valid URL");
    }
}