using CommandLine;
using JsonToClassConverter.ClassDefinitions;
using JsonToClassConverter.JsonParsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            await Host.CreateDefaultBuilder(args)
               .UseContentRoot(Directory.GetCurrentDirectory())
               .ConfigureServices((hostContext, services) =>
               {
                   CommandLineOptions commandLineOptions = new CommandLineOptions();

                   Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed(args =>
                   {
                       Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
                       .MinimumLevel.Override("System.Net.Http", Serilog.Events.LogEventLevel.Warning)// Suppress detailed HTTP logs
                       .WriteTo.Console(outputTemplate: "{Message}{NewLine}{Exception}")
                       .CreateLogger();

                       SetPathArgs(commandLineOptions, args);
                   });

                   services.Configure((Action<ConsoleLifetimeOptions>)(options => options.SuppressStatusMessages = true));

                   services.AddHttpClient();

                   services.AddSingleton(commandLineOptions)
                   .AddSingleton<IJsonParser, JsonParser>()
                   .AddSingleton<IJsonService, JsonService>()
                   .AddSingleton<IClassDefinitionGenerator, ClassDefinitionGenerator>()
                   .AddSingleton<IConverterController, ConverterController>()
                   .AddHostedService<ConsoleHostedService>();
               })
               .UseSerilog()
               .RunConsoleAsync();
        }
        catch (PathException ex)
        {
            Log.Logger.Fatal("There was an error on Startup when accessing path: {@Path}", ex.Path);
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal($"There was a fatal error on Startup {ex.Message}");
        }
    }

    private static void SetPathArgs(CommandLineOptions commandLineOptions, CommandLineOptions options)
    {
        commandLineOptions.ValidateArgs(options);

        commandLineOptions.FilePath = options.FilePath;
        commandLineOptions.JsonText = options.JsonText;
        commandLineOptions.Url = options.Url;

        options.OutputPath.CreateIfNotExists();
        commandLineOptions.OutputPath = options.OutputPath;
    }
}