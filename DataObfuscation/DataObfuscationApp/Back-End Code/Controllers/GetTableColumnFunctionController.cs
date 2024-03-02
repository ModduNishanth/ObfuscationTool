using Microsoft.AspNetCore.Mvc;
using DOA.Repository.Interface;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTableColumnFunctionController : ControllerBase
    {
        private readonly IGetTableColumnFunction _getTableColumnFunction;
        public GetTableColumnFunctionController(IGetTableColumnFunction getTableColumnFunction)
        {
            _getTableColumnFunction = getTableColumnFunction;
        }
        [HttpPost("GetTables")]
        //[ClientAuthorize]
        public async Task<ActionResult<List<string>>> GetTableName(Update update)
        {
            return await _getTableColumnFunction.GetTableNames(update);
        }

        [HttpPost("Getcolumns")]
        //[ClientAuthorize]
        public async Task<bool> GetColumnNames(GetTableColumn request)
        {
            return await _getTableColumnFunction.GetColumnNames(request);
        }
        [HttpPost("GetMappedColumnNames")]
        //[ClientAuthorize]
        public async Task<List<ColumnMapping>> GetMappedColumnNames(GetTableColumn request)
        {
            return await _getTableColumnFunction.GetMappedColumnNames(request);
        }

        [HttpPost("DeletedColumn")]
        //[ClientAuthorize]
        public async Task<bool> CheckIsColumnDeleted(GetTableColumn request)
        {
            return await _getTableColumnFunction.CheckIsColumnDeleted(request);
        }

        [HttpPost("GetObfuscationFunctionQuery")]
        //[ClientAuthorize]
        public async Task<List<FunctionInfo>> GetFunctionInfo(Update request)
        {
            return await _getTableColumnFunction.GetFunctionInfo(request);
        }

        [HttpGet("GetViewInfo")]
        //[ClientAuthorize]
        public async Task<List<FunctionInfo>> GetViewInfo()
        {
            return await _getTableColumnFunction.GetViewInfo();
        }

        [HttpGet("GetFunctionNames")]
        //[ClientAuthorize]
        public async Task<List<Function>> GetFunctionNames()
        {
            return await _getTableColumnFunction.GetFunctionNames();
        }

    }
}
