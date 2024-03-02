using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class ColumnMapping
    {
        public string ColumnName { get; set; }
        public int IsSelected { get; set; }
        public int FunctionId { get; set; }
        public string ConstantValue { get; set; }
    }
}
