using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class KeywordList
    {
        public int ProjectId { get; set; }
        public string? KeyWords { get; set; }

        public string? EditKeyword { get; set; }
        public int? KeywordId { get; set; }

        public string? DeleteKeyword { get; set; }
        public int? FunctionId { get; set; }
        public string? DisplayName { get; set; }
    }
}
