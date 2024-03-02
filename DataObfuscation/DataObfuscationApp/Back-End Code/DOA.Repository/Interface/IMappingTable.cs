using Models.Common;

namespace DOA.Repository.Interface
{
    public interface IMappingTable
    {
        Task<List<string>> MapTableColumn(GetTableColumn column);
        Task<List<IsObfuscated>> MappedTables(Update update);
        Task<byte[]> GetTableDataInExcelFormat(Update update);
        Task<List<vwGetTableColumnData>> GetAllTableColumnData(GetTableColumn getTableColumn);
        Task<bool> AddFunctionNo(FunctionAdd functionAdd);
        Task<bool> InsertStgMapping(ExecUpdateQueryObfuscationModel request);
        //Task<List<ColumnInfo>> ExecuteSPAndGetColumnInfo(ExecUpdateQueryObfuscationModel request);
        Task<List<Models.Common.TableColumnInfo>> ExecuteSPAndGetColumnInfo(ExecUpdateQueryObfuscationModel request);
    }
}
