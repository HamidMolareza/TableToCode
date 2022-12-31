using OnRail;
using OnRail.Extensions.Try;

namespace TableToCode.Helpers;

public static class Utility {
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