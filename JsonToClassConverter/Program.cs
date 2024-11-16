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
               .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location))               
               .ConfigureServices((hostContext, services) =>
               {
                   CommandLineOptions commandLineOptions = new CommandLineOptions();

                   Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed(options =>
                   {
                       Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
                       .WriteTo.Console(outputTemplate: "{Message}{NewLine}{Exception}")
                       .CreateLogger();

                       if (!string.IsNullOrEmpty(options.InputPath) && !string.IsNullOrEmpty(options.JsonText))
                           throw new ArgumentException("You must not use both -i and -j args together");
                       else if (string.IsNullOrEmpty(options.InputPath) && string.IsNullOrEmpty(options.JsonText))
                           throw new ArgumentException("You must use either -i or -j args, but not both");

                       commandLineOptions.InputPath = options.InputPath;
                       commandLineOptions.JsonText = options.JsonText;

                       options.OutputPath.CreateIfNotExists();
                       commandLineOptions.OutputPath = options.OutputPath;                       
                   });

                   services.Configure((Action<ConsoleLifetimeOptions>)(options => options.SuppressStatusMessages = true));

                   services.AddSingleton(commandLineOptions)
                   .AddSingleton<IJsonParser, JsonParser>()
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
            Log.Logger.Fatal("There was a fatal error on Startup {@ExceptionMessage}", ex.Message);
        }
    }
}