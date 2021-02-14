using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.ProcessExecution
{
    public interface IExecutor
    {
        public event DataReceivedEventHandler DataReceived;
        public event DataReceivedEventHandler ErrorReceived;

        Task<int> RunProcessAsync(string fileName, string args);
    }
}
