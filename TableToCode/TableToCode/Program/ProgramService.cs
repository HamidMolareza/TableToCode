using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using OnRail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using Sharprompt;
using TableToCode.Helpers;
using TableToCode.Models;
using TableToCode.TableData;
using TableToCode.TableDefinition;

namespace TableToCode.Program;

public class ProgramService : IProgram {
    private readonly ITableDefinition _tableDefinition;
    private readonly ILogger<ProgramService> _logger;
    private readonly ITableData _tableData;
    private readonly Configs _configs;
    private readonly Table _table = new();
    private string? _targetLanguage;

    public ProgramService(ILogger<ProgramService> logger, ITableDefinition tableDefinition, ITableData tableData,
        Configs configs) {
        _configs = configs;
        _logger = logger;
        _tableDefinition = tableDefinition;
        _tableData = tableData;
    }

    public Result Run() =>
        Utility.GetTableFromConsole("Input definition table  (without header row): ")
            .OnSuccess(tableRows => _tableDefinition.Parse(tableRows)
                .OnSuccessTee(tableColumns => _table.Columns = tableColumns)
            )
            .OnSuccess(() => Utility.GetTableFromConsole("Input table data (without column names): ")
                .OnSuccess(_tableData.Parse)
                .OnSuccess(tableData => _table.Data = tableData)
            ).OnSuccess(() => GetValidTableName()
                .OnSuccess(tableName => _table.Name = tableName)
            ).OnSuccess(() => GetTargetLanguage()
                .OnSuccess(targetLanguage => _targetLanguage = targetLanguage)
            )
            .OnSuccess(() => _tableDefinition.GenerateScript(_table.Name, _table.Columns, _targetLanguage!)
                .OnSuccess(script => Console.WriteLine(script))
            ).OnSuccess(() => _tableData.GenerateScript(_table.Name, _table.Data, _targetLanguage!, _table.Columns)
                .OnSuccess(script => Console.WriteLine(script)));

    private Result<string> GetValidTableName() {
        var regex = new Regex(_configs.ValidTableNameRegex);
        while (true) {
            Console.Write("Table name: ");
            var tableName = Console.ReadLine();

            if (string.IsNullOrEmpty(tableName)) {
                Console.WriteLine("Table name can not be empty.\n");
                continue;
            }

            var match = regex.Match(tableName);
            if (tableName.Length == match.Length)
                return Result<string>.Ok(tableName);

            Console.WriteLine($"Table name is not valid. Input must mach {_configs.ValidTableNameRegex}\n");
        }
    }

    private static Result<string> GetTargetLanguage() =>
        TryExtensions.Try(() => Prompt.Select("Target Language", new[] {"csharp", "postgres"}));
}