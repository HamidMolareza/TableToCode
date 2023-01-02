using System.Text;
using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.OnFail;
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
        }).OnFailAddMoreDetails(message);

    public static Result<string> ConvertToCamelCase(this string str) =>
        TryExtensions.Try(() => {
            var sb = new StringBuilder(str);

            var matches = Regex.Matches(str, "_[a-zA-Z]");
            for (var i = 0; i < matches.Count; i++) {
                var match = matches[i];
                var targetIndex = match.Index - i;
                sb.Remove(targetIndex, 2)
                    .Insert(targetIndex, char.IsLetter(sb[targetIndex])
                        ? $"{char.ToUpper(match.Value[1])}"
                        : $"{match.Value[1]}");
            }

            if (char.IsLetter(sb[0]) && char.IsLower(sb[0]))
                sb[0] = char.ToUpper(sb[0]);
            return sb.ToString();
        }).OnFailAddMoreDetails(str);
}