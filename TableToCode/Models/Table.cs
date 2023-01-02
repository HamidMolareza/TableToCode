namespace TableToCode.Models;

public class Table {
    public string Name { get; set; }
    public List<TableColumn> Columns { get; set; }
    public List<List<string>> Data { get; set; }
}