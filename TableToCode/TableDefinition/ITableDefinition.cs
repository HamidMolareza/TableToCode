using OnRail;
using TableToCode.Models;

namespace TableToCode.TableDefinition;

public interface ITableDefinition {
    Result<List<TableColumn>> Parse(List<string> tableRows);
    Result<string> GenerateScript(string tableName, List<TableColumn> tableColumns, string language);
}