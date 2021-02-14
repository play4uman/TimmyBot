using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.ProcessExecution
{
    public class Executor : IExecutor
    {
        public event DataReceivedEventHandler DataReceived;
        public event DataReceivedEventHandler ErrorReceived;

        public async Task<int> RunProcessAsync(string fileName, string args)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName, Arguments = args,
                    UseShellExecute = false, CreateNoWindow = true,
                    RedirectStandardOutput = true, RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };
            return await RunProcessAsync(process).ConfigureAwait(false);
        }
        private Task<int> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            process.Exited += (s, ea) => tcs.SetResult(process.ExitCode);
            process.OutputDataReceived += (s, ea) => DataReceived?.Invoke(s, ea);
            process.ErrorDataReceived += (s, ea) => ErrorReceived?.Invoke(s, ea);

            bool started = process.Start();
            if (!started)
            {
                //you may allow for the process to be re-used (started = false) 
                //but I'm not sure about the guarantees of the Exited event in such a case
                throw new InvalidOperationException("Could not start process: " + process);
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

    }
}
