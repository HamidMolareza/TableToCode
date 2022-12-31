using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using OnRail;
using OnRail.Extensions.OnSuccess;
using TableToCode.ErrorDetails;
using TableToCode.Helpers;

namespace TableToCode.DataTable;

public class DataTableService : IDataTable {
    private readonly IConfiguration _config;

    public DataTableService(IConfiguration configuration) {
        _config = configuration;
    }

    public Result<List<List<string>>> Parse(List<string> tableRows) =>
        Utility.GetDataDetectorRegex(_config)
            .OnSuccess(regexPattern => {
                var regex = new Regex(regexPattern);
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
                }

                return Result<List<List<string>>>.Ok(data);
            });
}