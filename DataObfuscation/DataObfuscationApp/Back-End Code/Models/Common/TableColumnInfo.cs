using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class TableColumnInfo
    {
        public int ProjectId { get; set; }
        public string TableName { get; set; }
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
    }
}

