using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.Try;
using OnRail.ResultDetails;

namespace TableToCode.Header;

public class HeaderParser : IHeaderParser {
    public Result<List<TableColumn>> ParseHeader(List<string> tableRows) =>
        TryExtensions.Try(() => {
            var regex = new Regex("(('|\")[a-zA-Z0-9_ ]+('|\"))|([a-zA-Z0-9_()])+"); //TODO: Get from config
            var columns = new List<TableColumn>();

            foreach (var tableRow in tableRows) {
                var matches = regex.Matches(tableRow);
                if (!matches.Any()) continue;
                if (matches.Count != 2) {
                    return Result<List<TableColumn>>.Fail(new ErrorDetail("Parse Header Error",
                        message:
                        $"Two columns are expected. Column name and type. But {matches.Count} columns have been identified.",
                        moreDetails: new {tableRow}));
                }

                columns.Add(new TableColumn(matches[0].Value, matches[1].Value));
            }

            return Result<List<TableColumn>>.Ok(columns);
        });
}