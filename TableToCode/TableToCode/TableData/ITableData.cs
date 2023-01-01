using OnRail;

namespace TableToCode.TableData;

public interface IDataTable {
    public Result<List<List<string>>> Parse(List<string> tableRows);
}