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

                       commandLineOptions.InputPath = options.InputPath;

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
            Log.Logger.Fatal("There was an error on Startup when accessing path: {@Path}. Exiting application", ex.Path);
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal("There was a fatal error on Startup {@ExceptionMessage} Exiting application", ex.Message);
        }
    }
}