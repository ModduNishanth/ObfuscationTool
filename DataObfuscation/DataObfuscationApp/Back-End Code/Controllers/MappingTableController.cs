using DOA.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MappingTableController : ControllerBase
    {
        private readonly IMappingTable _obfuscationFunction;
        public MappingTableController(IMappingTable obfuscationFunction)
        {
            _obfuscationFunction = obfuscationFunction;
        }
        [HttpPost("MapTableColumnValues")]
        //[ClientAuthorize]
        public async Task<List<string>> MapTableColumn(GetTableColumn column)
        {
            return await _obfuscationFunction.MapTableColumn(column);
        }
        [HttpPost("GetMappedTables")]
        //[ClientAuthorize]
        public async Task<ActionResult<List<IsObfuscated>>> GetMappedTables(Update update)
        {
            return await _obfuscationFunction.MappedTables(update);
        }
        [HttpPost("api/excel/download")]
        //[ClientAuthorize]
        public async Task<IActionResult> DownloadExcelFile(Update update)
        {
            try
            {
                byte[] excelData = await _obfuscationFunction.GetTableDataInExcelFormat(update);
                // Generate a unique file name for the Excel file
                string fileName = $"ObfuscationTable_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                // Set the content type and headers for the response
                Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Return the Excel file as the response
                return File(excelData, Response.ContentType, fileName);
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("vwGetTableColumnData")]
        //[ClientAuthorize]
        public async Task<List<vwGetTableColumnData>> GetAllTableColumnData(GetTableColumn getTableColumn)
        {
            return await _obfuscationFunction.GetAllTableColumnData(getTableColumn);
        }

        [HttpPost("AddFunctionNo")]
        //[ClientAuthorize]
        public async Task<bool> AddFunctionNo(FunctionAdd functionAdd)
        {
            return await _obfuscationFunction.AddFunctionNo(functionAdd);
        }

        [HttpPost("GetColumnName&DataType")]
        //public async Task<List<ColumnInfo>> ExecuteSPAndGetColumnInfo(ExecUpdateQueryObfuscationModel request)
        public async Task<List<Models.Common.TableColumnInfo>> ExecuteSPAndGetColumnInfo(ExecUpdateQueryObfuscationModel request)
        {
            return await _obfuscationFunction.ExecuteSPAndGetColumnInfo(request);
        }
    }
}
