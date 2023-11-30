using Authentication.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SettingModels;

internal class Program
{
    static async Task Main()
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
}