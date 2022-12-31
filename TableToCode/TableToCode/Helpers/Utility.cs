using Microsoft.Extensions.Configuration;
using OnRail;
using OnRail.Extensions.Try;
using TableToCode.ErrorDetails;

namespace TableToCode.Helpers;

public static class Utility {
    public static Result<string> GetDataDetectorRegex(IConfiguration config) =>
        TryExtensions.Try(() => {
            const string keyName = "DataDetectorRegex";
            var regexPattern = config[keyName];
            return string.IsNullOrEmpty(regexPattern)
                ? Result<string>.Fail(new ConfigurationError(message: $"{keyName} key is null."))
                : Result<string>.Ok(regexPattern);
        });
    
    public static Result<List<string>> GetTableFromConsole(string message) =>
        TryExtensions.Try(() => {
            Console.WriteLine(message);
            var tableRows = new List<string>();
            while (true) {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    return tableRows;
                tableRows.Add(line);
            }
        });
}