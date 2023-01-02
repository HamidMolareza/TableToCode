using OnRail;
using TableToCode.Models;

namespace TableToCode.TableData;

public interface ITableData {
    public Result<List<List<string>>> Parse(List<string> tableRows);

    public Result<string> GenerateScript(string tableName, List<List<string>> tableData,
        string targetLanguage, List<TableColumn> tableColumns);
}