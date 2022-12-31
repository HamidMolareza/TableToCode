using OnRail;
using TableToCode.Models;

namespace TableToCode.DefinitionTable;

public interface IDefinitionTable {
    Result<List<TableColumn>> Parse(List<string> tableRows);
}