using OnRail;

namespace TableToCode.DataTable;

public interface IDataTable {
    public Result<List<List<string>>> Parse(List<string> tableRows);
}