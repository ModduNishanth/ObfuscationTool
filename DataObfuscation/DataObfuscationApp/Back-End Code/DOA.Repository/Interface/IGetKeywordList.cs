using DOA.Repository.Implementation;
using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOA.Repository.Interface
{
    public interface IGetKeywordList
    {
        Task<List<KeywordList>> GetKeywordListAsync(Update request);
        Task<bool> UpdateKeywordList(KeywordList request);

        Task<bool> EditKeywordsAsync(KeywordList request);
        Task<int> DeleteKeywordsAsync(KeywordList request);
    }
}
