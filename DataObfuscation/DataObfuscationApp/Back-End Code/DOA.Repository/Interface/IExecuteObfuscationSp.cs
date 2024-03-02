using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOA.Repository.Interface
{
    public interface IExecuteObfuscationSp
    {
        Task<ExecutionResult> ExecUpdateQueryObfuscation(ExecUpdateQueryObfuscationModel request);
        Task<bool> CheckFunctionInfo(FunctionCheck request);
        Task<bool> UpdateObfuscateColumn(Update update);
        Task<bool> DeleteTable(Deletetable request);
    }
}
