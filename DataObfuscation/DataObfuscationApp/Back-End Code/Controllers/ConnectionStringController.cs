using DataObfuscationApp.Model;
using Microsoft.AspNetCore.Mvc;
using DOA.Repository.Interface;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionStringController : ControllerBase
    {
        private readonly IConnectionString _connectionString;
        public ConnectionStringController(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }
        [HttpPost("CreateConnectionString")]
        //[ClientAuthorize]
        public async Task<bool> CreateProject(CreateConnectionStringRequest request)
        {
            return await _connectionString.CreateProject(request);
        }
        [HttpPut("EditConnectionString")]
        //[ClientAuthorize]
        public async Task<bool> EditProject(int projectId, CreateConnectionStringRequest request)
        {
            return await _connectionString.EditProject(projectId ,request);
        }
        [HttpGet("TestConnectionString")]
        //[ClientAuthorize]
        public async Task<ResponseModel<bool>> TestAndCheckConnectionString(int projectId)
        {
            return await _connectionString.TestAndCheckConnectionString(projectId);
        }
        [HttpGet("GetProject")]
        //[ClientAuthorize]
        public async Task<List<CreateConnectionStringRequest>> GetAllProjects()
        {
            return await _connectionString.GetAllProjects();
        }
        [HttpPost("DeleteProject")]
        //[ClientAuthorize]
        public async Task<bool> DeleteProject(Update request)
        {
            return await _connectionString.DeleteProject(request);
        }

        [HttpGet("GetServerTypes")]
        //[ClientAuthorize]
        public async Task<List<GetServerTypeName>> GetServerType()
        {
            return await _connectionString.GetServerType();
        }
    }
}
