using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm.BERT
{
    public class BertWrapper : IBertWrapper, IDisposable
    {
        public BertWrapper()
        {
            _bertProcessHandle = new Process
            {
                StartInfo =
                {
                    FileName = command, Arguments = bertFile,
                    UseShellExecute = false, CreateNoWindow = true,
                    RedirectStandardOutput = true, RedirectStandardError = true, RedirectStandardInput = true
                },
                EnableRaisingEvents = true
            };
        }

        private Process _bertProcessHandle;
        private StreamWriter _processInput;
        private const string command = "python";
        private const string bertFile = @"..\AnswerExtraction\Algorithm\BERT\TimmyBot_BERT.py"; // todo: bake the py script in the library's dll so we don't have to do this

        public async Task InitializeAsync()
        {
            _bertProcessHandle.Start();
            _bertProcessHandle.BeginOutputReadLine();
            _bertProcessHandle.BeginErrorReadLine();
            _processInput = _bertProcessHandle.StandardInput;

            bool dataReceived = false;
            _bertProcessHandle.ErrorDataReceived += (s, ea) => throw new ApplicationException($"BERT script ecountered an error: {ea.Data}");
            DataReceivedEventHandler onReadyReceived = (s, ea) =>
            {
                if (ea.Data == "READY")
                    dataReceived = true;
            };
            _bertProcessHandle.OutputDataReceived += onReadyReceived;

            while (!dataReceived)
                await Task.Delay(100);

            _bertProcessHandle.OutputDataReceived -= onReadyReceived;
        }

        public async Task<string> GetAnswerAsync(string question, string paragraph, bool debug = false)
        {
            if (debug)
            {
                question = "Why was the student group called \"the Methodists\"?";
                paragraph = "The movement which would become The United Methodist Church began in the mid-18th century within the Church of England. A small group of students, including John Wesley, Charles Wesley and George Whitefield, met on the Oxford University campus. " +
                    "They focused on Bible study, methodical study of scripture and living a holy life. Other students mocked them, saying they were the \"Holy Club\" and \"the Methodists\", being methodical and exceptionally detailed in their Bible study, opinions and disciplined lifestyle. " +
                    "Eventually, the so-called Methodists started individual societies or classes for members of the Church of England who wanted to live a more religious life.";
            }


            string answer = null;
            bool dataReceived = false;
            _bertProcessHandle.OutputDataReceived += (s, ea) =>
            {
                answer = ea.Data.Split("ANSWER: ")[1];
                dataReceived = true;
            };

            _processInput.WriteLine(question);
            _processInput.WriteLine(paragraph);
            _processInput.Flush();


            while (!dataReceived)
                await Task.Delay(25);
            return answer;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _processInput.Dispose();
                _bertProcessHandle.Dispose();
            }
        }
    }
}
