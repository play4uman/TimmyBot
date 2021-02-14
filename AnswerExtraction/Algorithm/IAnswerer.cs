﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerExtraction.Algorithm
{
    public interface IAnswerer
    {
        Task<string> AnswerAsync(string question, string subject);
    }
}
