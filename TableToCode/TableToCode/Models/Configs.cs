namespace TableToCode.Models;

public class Configs {
    public string DataDetectorRegex { get; set; } = null!;
    public string ValidTableNameRegex { get; set; } = null!;
    public string BetweenParenthesesRegex { get; set; } = null!;
    public List<KeyValuePair<string, string>> ConvertTypeToCsharp { get; set; } = null!;
}