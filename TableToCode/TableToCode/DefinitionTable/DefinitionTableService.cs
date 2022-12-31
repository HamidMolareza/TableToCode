using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.Try;
using TableToCode.ErrorDetails;
using TableToCode.Models;

namespace TableToCode.DefinitionTable;

public class DefinitionTableService : IDefinitionTable {
    private readonly Configs _config;

    public DefinitionTableService(Configs configs) {
        _config = configs;
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
}