using DOA.Repository.Implementation;
using DOA.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObfuscationController : ControllerBase
    {
        private readonly IExecuteObfuscationSp _executeObfuscationSp;
        public ObfuscationController(IExecuteObfuscationSp executeObfuscationSp)
        {
            _executeObfuscationSp = executeObfuscationSp;
        }
     
        [HttpPost("UpdateObfuscationQuery")]
        //[ClientAuthorize]
        public async Task<ActionResult<ExecutionResult>> ExecUpdateQueryObfuscation(ExecUpdateQueryObfuscationModel request)
        {
            return await _executeObfuscationSp.ExecUpdateQueryObfuscation(request);
        }

        [HttpPost("CheckFunctionInfo")]
        //[ClientAuthorize]
        public async Task<ActionResult<bool>> CheckFunctionInfo(FunctionCheck request)
        {
            return await _executeObfuscationSp.CheckFunctionInfo(request);
        }

        [HttpPost("UpdateObfuscateColumn")]
        //[ClientAuthorize]
        public async Task<bool> UpdateObfuscateColumn(Update update)
        {
            return await _executeObfuscationSp.UpdateObfuscateColumn(update);
        }

        [HttpPost("DeleteTable")]
        //[ClientAuthorize]
        public async Task<bool> DeleteTable(Deletetable request)
        {
            return await _executeObfuscationSp.DeleteTable(request);
        }
    }
}
