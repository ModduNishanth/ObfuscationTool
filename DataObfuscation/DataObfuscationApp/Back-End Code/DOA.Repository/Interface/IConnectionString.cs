using DataObfuscationApp.Model;
using Microsoft.AspNetCore.Http;
using Models.Common;

namespace DOA.Repository.Interface
{
    public interface IConnectionString
    {
        Task<bool> CreateProject(CreateConnectionStringRequest request);
        Task<bool> EditProject(int projectId, CreateConnectionStringRequest request);
        Task<ResponseModel<bool>> TestAndCheckConnectionString(int projectId);
        Task<List<CreateConnectionStringRequest>> GetAllProjects();
        Task<bool> DeleteProject(Update request);
        public Task<List<GetServerTypeName>> GetServerType();
    }
}
