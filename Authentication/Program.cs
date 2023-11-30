using Authentication.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SettingModels;

internal class Program
{
    static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog();
        });
        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

        // Log some messages with different log levels and message templates
        logger.LogTrace("This is a trace message.");
        logger.LogDebug("This is a debug message.");
        logger.LogInformation("Hello {Name}!", "World");
        logger.LogWarning("This is a warning message.");
        logger.LogError("This is an error message.");
        logger.LogCritical("This is a critical message.");


        try
        {

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var jsonFilePath = $"appsettings.{environment}.json";
            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"Warning: The configuration file '{jsonFilePath}' does not exist.");
            }

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(jsonFilePath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var rabbitMqSetting = configuration.GetSection(nameof(RabbitMqSetting)).Get<RabbitMqSetting>();

            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddSerilog())
                .AddMassTransitExtension(rabbitMqSetting!.Host, rabbitMqSetting.Username, rabbitMqSetting.Password);

            // Start MassTransit asynchronously
            services = await services.StartMassTransitAsync();

            services.BuildServiceProvider();

            // Run your application logic here
            Console.WriteLine("MassTransit consumer is running. Press Ctrl+C to exit.");

            // Keep the program alive until terminated
            var exitEvent = new ManualResetEventSlim();
            Console.CancelKeyPress += (sender, eventArgs) => exitEvent.Set();

            // Block the main thread to keep the program running
            exitEvent.Wait();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}