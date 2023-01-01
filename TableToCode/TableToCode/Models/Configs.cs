using TableToCode.TypeConverter;

namespace TableToCode.Models;

public class Configs {
    public string DataDetectorRegex { get; set; } = null!;
    public string ValidTableNameRegex { get; set; } = null!;
    public string BetweenParenthesesRegex { get; set; } = null!;
    public List<ConvertTypesModel> ConvertTypeToCsharp { get; set; } = null!;
}