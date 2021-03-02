using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dikubot.Webapp.controllers
{
    public class code : Controller
    {
        // GET
        public string Index()
        {
            return "This is my default action...";
        }
        

        private static string _codeInMemoryStore;
    
        [HttpGet]
        public ActionResult<string> GetCode()
        {
            return _codeInMemoryStore;
        }

        [HttpGet("{code}")]
        public ActionResult<string> GetById(string code)
        {
            return code;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> SetCode(string code)
        {
            _codeInMemoryStore = code;
            return _codeInMemoryStore;
        }

    }
    
}