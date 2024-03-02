using DOA.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreviewController : ControllerBase
    {
        private readonly IPreviewData _previewData;

        public PreviewController(IPreviewData previewData)
        {
            _previewData = previewData;
        }
        [HttpPost("GetPreviewquery")]
        //[ClientAuthorize]
        public async Task<ActionResult<List<string>>> GetUpdatePreview([FromBody] Obfuscation request)
        {
            return await _previewData.GetPreviewUpdate(request);
        }
        [HttpPost("ViewPreviewQuery")]
        //[ClientAuthorize]
        public async Task<List<string>> ExecutePreviewQuery(SelectQuery request)
        {
            return await _previewData.ExecutePreviewQuery(request);
        }
    }
}
