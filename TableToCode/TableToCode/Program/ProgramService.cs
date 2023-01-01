using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using OnRail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using Sharprompt;
using TableToCode.DataTable;
using TableToCode.DefinitionTable;
using TableToCode.Helpers;
using TableToCode.Models;

namespace TableToCode.Program;

public class ProgramService : IProgram {
    private readonly IDefinitionTable _definitionTable;
    private readonly ILogger<ProgramService> _logger;
    private readonly IDataTable _dataTable;
    private readonly Configs _configs;
    private readonly Table _table = new();
    private string? _targetLanguage;

    public ProgramService(ILogger<ProgramService> logger, IDefinitionTable definitionTable, IDataTable dataTable,
        Configs configs) {
        _configs = configs;
        _logger = logger;
        _definitionTable = definitionTable;
        _dataTable = dataTable;
    }

    public Result Run() =>
        Utility.GetTableFromConsole("Input definition table: ")
            .OnSuccess(tableRows => _definitionTable.Parse(tableRows)
                .OnSuccessTee(tableColumns => _table.Columns = tableColumns)
            )
            .OnSuccess(Utility.GetTableFromConsole("Input table data (without column names): ")
                .OnSuccess(_dataTable.Parse)
                .OnSuccess(tableData => _table.Data = tableData)
            ).OnSuccess(() => GetValidTableName()
                .OnSuccess(tableName => _table.Name = tableName)
            ).OnSuccess(() => GetTargetLanguage()
                .OnSuccess(targetLanguage => _targetLanguage = targetLanguage)
            )
            .OnSuccess(() => _definitionTable.GenerateScript(_table.Name, _table.Columns, _targetLanguage!)
                .OnSuccess(script => Console.WriteLine(script))
            );

    private Result<string> GetValidTableName() {
        var regex = new Regex(_configs.ValidTableNameRegex);
        while (true) {
            Console.Write("Table name: ");
            var tableName = Console.ReadLine();
            if (!string.IsNullOrEmpty(tableName) && regex.IsMatch(tableName)) {
                return Result<string>.Ok(tableName);
            }

            Console.WriteLine($"Table name is not valid. Input must mach {_configs.ValidTableNameRegex}.\n");
        }
    }

    private static Result<string> GetTargetLanguage() =>
        TryExtensions.Try(() => Prompt.Select("Target Language", new[] {"csharp", "postgres"}));
}