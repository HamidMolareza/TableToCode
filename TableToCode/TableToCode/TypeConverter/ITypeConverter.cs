using OnRail;

namespace TableToCode.TypeConverter;

public interface ITypeConverter {
    public Result<string> Convert(string type, string targetLanguageName);
}