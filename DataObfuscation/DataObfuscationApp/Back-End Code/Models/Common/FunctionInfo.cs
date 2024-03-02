using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class FunctionInfo
    {
        public string FunctionName { get; set; }
        public string FunctionDefinition { get; set; }

        public FunctionInfo(string functionName, string functionDefinition)
        {
            FunctionName = functionName;
            FunctionDefinition = functionDefinition;
        }
    }

}
