using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnRail;
using OnRail.Extensions.OnFail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using TableToCode.Header;
using TableToCode.Program;

namespace TableToCode;

public static class Startup {
    public static Task Main(string[] args) =>
        TryExtensions.Try(() => InnerMainAsync(args))
            .OnSuccess(() => Console.WriteLine("The operation was completed successfully."))
            .OnFail(result => {
                //TODO: Log Error
                Console.WriteLine($"{result.Detail?.Title}\n{result.Detail?.Message}");
                return result;
            });

    private static async Task InnerMainAsync(string[] args) {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services)
                => services
                    .AddSingleton<IConfiguration>(configuration)
                    .AddLogging(loggingBuilder => loggingBuilder
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddConsole())
                    .AddScoped<IProgram, ProgramService>()
                    .AddScoped<IHeaderParser, HeaderParser>())
            .Build();

        RunProgram(host.Services)
            .OnFail(result => {
                //TODO: Log Error
                Console.WriteLine($"{result.Detail?.Title}\n{result.Detail?.Message}");
                return result;
            });

        await host.RunAsync();
    }

    private static Result RunProgram(IServiceProvider services) {
        using var serviceScope = services.CreateScope();
        var provider = serviceScope.ServiceProvider;

        var program = provider.GetRequiredService<IProgram>();
        return program.Run();
    }
}