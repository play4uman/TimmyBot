using AnswerExtraction.Algorithm;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController
    {
        public QuestionController(IAnswerer answerer)
        {
            _answerer = answerer;
        }

        private readonly IAnswerer _answerer;
        [HttpGet]
        public async Task<string> GetAnswer(string q)
        {
            return await _answerer.AnswerAsync(q);
        }
    }
}
