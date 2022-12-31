﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnRail;
using OnRail.Extensions.OnFail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using OnRail.ResultDetails;
using TableToCode.DefinitionTable;
using TableToCode.Helpers;
using TableToCode.Program;

namespace TableToCode;

public static class Startup {
    public static void Main(string[] args) =>
        TryExtensions.Try(() => InnerMain(args))
            .OnSuccess(() => Console.WriteLine("The operation was completed successfully."))
            .OnFail(result => {
                Console.WriteLine(LogHelper.Log(result.Detail as ErrorDetail));
                return result;
            });

    private static void InnerMain(string[] args) {
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
                    .AddScoped<IDefinitionTable, DefinitionTableService>())
            .Build();

        RunProgram(host.Services)
            .OnFail(result => {
                Console.WriteLine(LogHelper.Log(result.Detail as ErrorDetail));
                return result;
            });
    }

    private static Result RunProgram(IServiceProvider services) {
        using var serviceScope = services.CreateScope();
        var provider = serviceScope.ServiceProvider;

        var program = provider.GetRequiredService<IProgram>();
        return program.Run();
    }
}