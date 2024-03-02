using DOA.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Common;

namespace DataObfuscationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyWordListController : ControllerBase
    {
        private readonly IGetKeywordList _KeywordList;
        public KeyWordListController(IGetKeywordList KeywordList)
        {
            _KeywordList = KeywordList;
        }
        [HttpPost("GetKeyWordcolumns")]
        //[ClientAuthorize]
        public async Task<List<KeywordList>> GetKeywordListAsync(Update request)
        {
            return await _KeywordList.GetKeywordListAsync(request);
        }
        [HttpPost("UpdateKeywordList")]
        //[ClientAuthorize]
        public async Task<bool> UpdateKeywordList(KeywordList request)
        {
            return await _KeywordList.UpdateKeywordList(request);
             
        }

        [HttpPut("EditKeyword")]
        //[ClientAuthorize]
        public async Task<bool> EditKeywordsAsync(KeywordList request)
        {
            return await _KeywordList.EditKeywordsAsync(request);
        }

        [HttpDelete("DeleteteKeyword")]
        //[ClientAuthorize]
        public async Task<int> DeleteKeywordsAsync(KeywordList request)
        {
          return await _KeywordList.DeleteKeywordsAsync(request);         
        }
    }
}
