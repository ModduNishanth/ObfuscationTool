using DataObfuscationApp.Model;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace DOA.Repository.Interface
{
    public interface IUpdateFunction
    {
        Task<bool> CreateFunctions(UpdateFunctions request);
        Task<bool> CopyProject(CopyProject copyProject);
    }
}
