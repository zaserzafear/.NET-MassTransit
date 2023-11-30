using Authentication.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SettingModels;

internal class Program
{
    private static async Task Main()
    {
        ConfigureLogging();

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());

        try
        {
            var environment = GetEnvironment();

            var configuration = BuildConfiguration(environment);

            var rabbitMqSetting = configuration.GetSection(nameof(RabbitMqSetting)).Get<RabbitMqSetting>()!;

            var services = BuildServiceCollection(rabbitMqSetting);

            services = await StartMassTransitAsync(services);

            using var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("MassTransit consumer is running. Press Ctrl+C to exit.");

            await WaitForExitAsync();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
    }

    private static string GetEnvironment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var jsonFilePath = $"appsettings.{environment}.json";

        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine($"Warning: The configuration file '{jsonFilePath}' does not exist.");
        }

        return environment;
    }

    private static IConfiguration BuildConfiguration(string environment)
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static IServiceCollection BuildServiceCollection(RabbitMqSetting rabbitMqSetting)
    {
        return new ServiceCollection()
            .AddLogging(builder => builder.AddSerilog())
            .AddMassTransitExtension(rabbitMqSetting!.Host, rabbitMqSetting.Username, rabbitMqSetting.Password);
    }

    private static async Task<IServiceCollection> StartMassTransitAsync(IServiceCollection services)
    {
        return await services.StartMassTransitAsync();
    }

    private static async Task WaitForExitAsync()
    {
        var exitEvent = new ManualResetEventSlim();
        Console.CancelKeyPress += (sender, eventArgs) => exitEvent.Set();
        await Task.Run(() => exitEvent.Wait());
    }
}
