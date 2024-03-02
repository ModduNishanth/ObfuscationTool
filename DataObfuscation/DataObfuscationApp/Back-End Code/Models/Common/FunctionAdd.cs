using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class FunctionAdd
    {
        public int ProjectID { get; set; }
        public string TableName { get; set; }
        public List<string> ColumnName { get; set; }

        public List<int> IsSelected { get; set; }
        public List<int> FunctionNo { get; set; }
        public List<string> ColumnStatus { get; set; }

        public List<string>? ConstantValue { get; set; }
        public List<string>? CertificateName { get; set; }

    }

}
