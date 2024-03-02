using Models.Common;

namespace DOA.Repository.Interface
{
    public interface IPreviewData
    {
        Task<List<string>> GetPreviewUpdate(Obfuscation request);
        Task<List<string>> ExecutePreviewQuery(SelectQuery request);
    }
}
