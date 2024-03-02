using Models.Common;

namespace DOA.Repository.Interface
{
    public interface IGetTableColumnFunction
    {
        Task<List<string>> GetTableNames(Update update);
        Task<bool> GetColumnNames(GetTableColumn request);
        Task<List<ColumnMapping>> GetMappedColumnNames(GetTableColumn request);
        Task<bool> CheckIsColumnDeleted(GetTableColumn request);
        Task<List<FunctionInfo>> GetFunctionInfo(Update request);
        Task<List<FunctionInfo>> GetViewInfo();
        Task<List<Function>> GetFunctionNames();
    }
}
