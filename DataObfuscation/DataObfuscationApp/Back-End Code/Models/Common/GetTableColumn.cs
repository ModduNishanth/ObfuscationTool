namespace Models.Common
{
    public class GetTableColumn
    {
        public int ProjectId { get; set; }
        public string? tableName { get; set; }
        public string? ColumnName { get; set; }
    }

    public class ColumnNames
    {
        public string columnName { get; set; }
        public bool isObfuscated { get; set; }
    }
}
