using OnRail;

namespace TableToCode.Header;

public interface IHeaderParser {
    Result<List<TableColumn>> ParseHeader(List<string> tableRows);
}