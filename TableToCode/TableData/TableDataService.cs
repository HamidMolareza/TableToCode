using System.Text;
using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using OnRail.ResultDetails;
using TableToCode.ErrorDetails;
using TableToCode.Helpers;
using TableToCode.Models;
using TableToCode.TypeConverter;

namespace TableToCode.TableData;

public class TableDataService : ITableData {
    private readonly Configs _config;
    private readonly ITypeConverter _typeConverter;

    public TableDataService(Configs configs, ITypeConverter typeConverter) {
        _config = configs;
        _typeConverter = typeConverter;
    }

    public Result<List<List<string>>> Parse(List<string> tableRows) =>
        TryExtensions.Try(() => {
            var regex = new Regex(_config.DataDetectorRegex);
            var data = new List<List<string>>();
            int? columnsCount = null;

            foreach (var tableRow in tableRows) {
                var matches = regex.Matches(tableRow);

                if (!matches.Any()) continue;
                if (columnsCount is not null && matches.Count != columnsCount) {
                    return Result<List<List<string>>>.Fail(new ParseError(message:
                        $"{columnsCount} columns are expected. But {matches.Count} columns have been identified.",
                        moreDetails: new {tableRow}));
                }

                data.Add(matches.Select(match => match.Value).ToList());
                columnsCount ??= matches.Count;
            }

            return Result<List<List<string>>>.Ok(data);
        });

    public Result<string> GenerateScript(string tableName, List<List<string>> tableData,
        string targetLanguage, List<TableColumn> tableColumns) =>
        targetLanguage.ToLower() switch {
            "postgres" => GenerateScriptForPostgres(tableName, tableData, tableColumns),
            "csharp" => GenerateScriptForCsharp(tableName, tableData, tableColumns),
            _ => Result<string>.Fail(new NotSupportedError($"{targetLanguage} is not supported."))
        };

    private Result<string> GenerateScriptForCsharp(string tableName, List<List<string>> tableData,
        IReadOnlyList<TableColumn> tableColumns) =>
        tableName.ConvertToCamelCase()
            .OnSuccess(tableNameCamelCase => {
                var dbSetName = $"{tableNameCamelCase}s";
                var itemsName = char.ToLower(dbSetName[0]) + dbSetName[1..];

                var sb = new StringBuilder();
                const string contextName = "ProblemContext";
                sb.Append($"public class {contextName}").AppendLine(" {")
                    .Append($"    public DbSet<{tableNameCamelCase}> {tableNameCamelCase}s")
                    .AppendLine(" { get; set; } = null!;")
                    .AppendLine()
                    .Append($"    public static async Task SeedDataAsync({contextName} db) ").AppendLine("{")
                    .Append($"        var {itemsName} = new {tableNameCamelCase}[]").AppendLine(" {");

                foreach (var row in tableData) {
                    var result = AppendItemForCsharp(sb, tableColumns, row);
                    if (!result.IsSuccess) return Result<string>.Fail(result.Detail as ErrorDetail);
                }

                sb.AppendLine("        };")
                    .AppendLine()
                    .AppendLine($"        db.{dbSetName}.AddRange({itemsName});")
                    .AppendLine("        await db.SaveChangesAsync();")
                    .AppendLine("    }")
                    .AppendLine("}");

                return Result<string>.Ok(sb.ToString());
            });

    private Result AppendItemForCsharp(StringBuilder sb,
        IReadOnlyList<TableColumn> tableColumns, IReadOnlyList<string> row) =>
        TryExtensions.Try(() => {
            sb.AppendLine("            new() {");
            for (var i = 0; i < tableColumns.Count; i++) {
                var result = tableColumns[i].ColumnName.ConvertToCamelCase()
                    .OnSuccess(columnNameCamelCase =>
                        GetValueFormattedForCsharp(row[i], tableColumns[i].ColumnType)
                            .OnSuccess(value => sb.AppendLine($"                {columnNameCamelCase} = {value},"))
                    );
                if (!result.IsSuccess)
                    return Result.Fail(result.Detail as ErrorDetail);
            }

            sb.AppendLine("            },");
            return Result.Ok();
        });

    private Result<string> GetValueFormattedForCsharp(string value, string type) =>
        _typeConverter.Convert(type, "csharp")
            .OnSuccess(csharpType => {
                if (value.ToLower() == "null")
                    return "null";
                switch (csharpType.ToLower()) {
                    case "long":
                    case "int":
                    case "double":
                    case "float":
                    case "decimal":
                    case "short":
                        return value;
                    case "string":
                        return $"\"{value}\"";
                    case "bool":
                        return value.ToLower();
                    case "date":
                        return $"new Date({value})";
                    case "guid":
                        return $"new Guid({value})";
                    default:
                        return
                            $"{value} /* Warning: The script generator not support this type. Ensure value format is valid. */";
                }
            });

    private static Result<string> GenerateScriptForPostgres(string tableName,
        IReadOnlyCollection<List<string>> tableData,
        List<TableColumn> tableColumns) =>
        TryExtensions.Try(tableName.ConvertToCamelCase)
            .OnSuccess(tableNameCamelCase => {
                var sb = new StringBuilder();
                sb.Append($"insert into {tableNameCamelCase} (")
                    .AppendLine($"{string.Join(", ", tableColumns.Select(column => column.ColumnName))})")
                    .Append("values ");

                var data = tableData.Select(row => {
                    var rowValues = new List<string>(row.Count);
                    for (var i = 0; i < row.Count; i++)
                        rowValues.Add(GetValueFormattedForPostgres(row[i], tableColumns[i].ColumnType));

                    return rowValues;
                });

                foreach (var row in data)
                    sb.AppendLine($"({string.Join(", ", row)}), ");

                //Remove last , 
                sb.Remove(sb.Length - 3, 3)
                    .AppendLine(";");

                sb.AppendLine()
                    .AppendLine("-- Remove Data")
                    .AppendLine($"DROP TABLE {tableNameCamelCase};");

                return sb.ToString();
            });

    private static string GetValueFormattedForPostgres(string value, string type) {
        return value.ToLower() == "null" ? "Null" : $"'{value}'";
    }
}