using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class Obfuscation
    {
        public string? tableName { get; set; }
        public int FunctionNo { get; set; }
        public string? FunctionName { get; set; }
        public string ColumnName { get; set; }
        public int ProjectId { get; set; }
        public object DataType { get; set; }

        public string? ConstantValue { get;set; }
    }

    public class MappingTableColumns
    {
        public string? tableName { get; set; }      
        public List<MappingColumn> mappingColumns { get; set; }
        public string ProjectNo { get; set; }
        public string? IsSelected { get; set; }
    }

    public class MappingColumn
    {
        public string ColumnName { get; set; }
    }

}
