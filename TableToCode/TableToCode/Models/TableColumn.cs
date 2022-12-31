namespace TableToCode.Models;

public class TableColumn {
    public TableColumn(string columnName, string columnType) {
        if (string.IsNullOrEmpty(columnName))
            throw new ArgumentNullException(nameof(columnName));
        if (string.IsNullOrEmpty(columnType))
            throw new ArgumentNullException(nameof(columnType));

        ColumnName = columnName;
        ColumnType = columnType;
    }

    public string ColumnName { get; set; }
    public string ColumnType { get; set; }
}