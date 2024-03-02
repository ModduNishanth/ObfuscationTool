using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class Execobfuscation
    {
        public string ProjectName { get; set; }
        public string TableNames { get; set; }
    }

    public class Deletetable
    {
        public int ProjectId { get; set; }
        public string TableName { get; set; }
    }
}
