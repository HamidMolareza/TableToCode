using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services)
                => services
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