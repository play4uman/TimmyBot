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
        [HttpGet]
        public async Task<string> GetAnswer(string q)
        {
            return "Hello there";
        }
    }
}
