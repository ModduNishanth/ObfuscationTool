using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class ExecutionResult
    {
        public bool IsSuccessful { get; set; }
        public string successfulMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string TableName { get; set; }
    }
}
