using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.Try;
using TableToCode.ErrorDetails;
using TableToCode.Models;

namespace TableToCode.TableData;

public class TableDataService : IDataTable {
    private readonly Configs _config;

    public TableDataService(Configs configs) {
        _config = configs;
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
}