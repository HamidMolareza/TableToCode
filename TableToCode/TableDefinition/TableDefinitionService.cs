using System.Text;
using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.OnFail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using OnRail.ResultDetails;
using TableToCode.ErrorDetails;
using TableToCode.Helpers;
using TableToCode.Models;
using TableToCode.TypeConverter;

namespace TableToCode.TableDefinition;

public class TableDefinitionService : ITableDefinition {
    private readonly Configs _config;
    private readonly ITypeConverter _typeConverter;

    public TableDefinitionService(Configs configs, ITypeConverter typeConverter) {
        _config = configs;
        _typeConverter = typeConverter;
    }

    public Result<List<TableColumn>> Parse(List<string> tableRows) =>
        TryExtensions.Try(() => {
            var regex = new Regex(_config.DataDetectorRegex);
            var columns = new List<TableColumn>();

            foreach (var tableRow in tableRows) {
                var matches = regex.Matches(tableRow);
                if (!matches.Any()) continue;
                if (matches.Count != 2) {
                    return Result<List<TableColumn>>.Fail(new ParseError(message:
                        $"Two columns are expected. Column name and type. But {matches.Count} columns have been identified.",
                        moreDetails: new {tableRow}));
                }

                columns.Add(new TableColumn(matches[0].Value, matches[1].Value));
            }

            return Result<List<TableColumn>>.Ok(columns);
        });

    public Result<string> GenerateScript(string tableName, List<TableColumn> tableColumns, string language) =>
        language.ToLower() switch {
            "postgres" => GenerateScriptForPostgres(tableName, tableColumns),
            "csharp" => GenerateScriptForCsharp(tableName, tableColumns),
            _ => Result<string>.Fail(new NotSupportedError($"{language} is not supported."))
        };

    private Result<string> GenerateScriptForPostgres(string tableName, List<TableColumn> tableColumns) =>
        TryExtensions.Try(tableName.ConvertToCamelCase)
            .OnSuccess(tableNameCamelCase => {
                var sb = new StringBuilder();
                sb.AppendLine("-- Create Items")
                    .AppendLine($"Create table If Not Exists {tableNameCamelCase}")
                    .AppendLine("(");

                foreach (var tableColumn in tableColumns) {
                    _typeConverter.Convert(tableColumn.ColumnType, "postgres")
                        .OnSuccessTee(type => sb.AppendLine($"\t{tableColumn.ColumnName}\t{type},"))
                        .OnFailThrowException();
                }

                sb.Remove(sb.Length - 2, 1); //Remove last ,

                sb.AppendLine(");")
                    .AppendLine($"Truncate table {tableNameCamelCase};");
                return sb.ToString();
            });

    private Result<string> GenerateScriptForCsharp(string tableName, List<TableColumn> tableColumns) =>
        TryExtensions.Try(() => {
            var sb = new StringBuilder();
            tableName.ConvertToCamelCase()
                .OnSuccess(tableNameCamelCase =>
                    sb.Append($"public class {tableNameCamelCase} ").AppendLine("{"));
            return sb;
        }).OnSuccess(sb => {
            foreach (var tableColumn in tableColumns) {
                var result = _typeConverter.Convert(tableColumn.ColumnType, "csharp")
                    .OnSuccessTee(type => tableColumn.ColumnName.ConvertToCamelCase()
                        .OnSuccess(columnName =>
                            sb.Append($"public {type} {columnName} ").AppendLine("{ get; set; }")));
                if (!result.IsSuccess)
                    return Result<string>.Fail(result.Detail as ErrorDetail);
            }

            sb.AppendLine("}");
            return Result<string>.Ok(sb.ToString());
        });
}