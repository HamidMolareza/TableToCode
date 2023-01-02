using System.Text.RegularExpressions;
using OnRail;
using OnRail.Extensions.OnSuccess;
using OnRail.Extensions.Try;
using TableToCode.Models;

namespace TableToCode.TypeConverter;

public class TypeConverterService : ITypeConverter {
    private readonly Configs _configs;

    public TypeConverterService(Configs configs) {
        _configs = configs;
    }

    public Result<string> Convert(string type, string targetLanguageName) =>
        TryExtensions.Try(() => {
            return targetLanguageName.ToLower() switch {
                "csharp" => ConvertToCsharp(type),
                _ => Result<string>.Ok(type)
            };
        });


    private Result<string> ConvertToCsharp(string type) =>
        RemoveParentheses(type.ToLower())
            .OnSuccess(pureType => {
                var findIndex = _configs.ConvertTypeToCsharp.FindIndex(keyValue => keyValue.Key == pureType);
                return findIndex >= 0
                    ? _configs.ConvertTypeToCsharp[findIndex].Value
                    : type;
            });

    private Result<string> RemoveParentheses(string type) =>
        TryExtensions.Try(() => {
            var regex = new Regex(_configs.BetweenParenthesesRegex);
            var match = regex.Match(type);
            return type.Remove(match.Index, match.Length);
        });
}