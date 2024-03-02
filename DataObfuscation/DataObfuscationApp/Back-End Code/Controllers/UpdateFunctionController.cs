using DataObfuscationApp.Model;
using DOA.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateFunctionController : ControllerBase
    {
        private readonly IUpdateFunction _updateFunction;

        public UpdateFunctionController(IUpdateFunction updateFunction)
        {
            _updateFunction = updateFunction;
        }
        [HttpPost("CreateFunctions")]
        //[ClientAuthorize]
        public async Task<bool> CreateFunctions([FromBody] UpdateFunctions request)
        {
            return await _updateFunction.CreateFunctions(request);
        }
        [HttpPost("CopyProject")]
        //[ClientAuthorize]
        public async Task<bool> CopyProjects(CopyProject copyProject)
        {
            return await _updateFunction.CopyProject(copyProject);
        }
    }
}
