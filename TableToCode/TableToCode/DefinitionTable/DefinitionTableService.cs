using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using OnRail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using TableToCode.ErrorDetails;
using TableToCode.Models;

namespace TableToCode.DefinitionTable;

public class DefinitionTableService : IDefinitionTable {
    private readonly IConfiguration _config;

    public DefinitionTableService(IConfiguration configuration) {
        _config = configuration;
    }

    public Result<List<TableColumn>> ParseHeader(List<string> tableRows) =>
        TryExtensions.Try(() => {
            const string keyName = "DataDetectorRegex";
            var regexPattern = _config[keyName];
            return string.IsNullOrEmpty(regexPattern)
                ? Result<string>.Fail(new ConfigurationError(message: $"{keyName} key is null."))
                : Result<string>.Ok(regexPattern);
        }).OnSuccess(regexPattern => {
            var regex = new Regex(regexPattern);
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
}