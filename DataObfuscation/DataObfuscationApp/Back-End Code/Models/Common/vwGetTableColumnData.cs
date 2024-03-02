using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class vwGetTableColumnData
    {
        public int ProjectID { get; set; }
        public int MappingID { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int? FunctionID { get; set; }
        public string MappingStatus { get; set; }
        public string DisplayName { get; set; }
        public byte[] IsObfuscated { get; set; }
    }
}
