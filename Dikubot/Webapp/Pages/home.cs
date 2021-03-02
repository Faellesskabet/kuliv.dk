using Microsoft.AspNetCore.Mvc;

namespace Dikubot.Webapp.Pages
{
    public class home : Controller
    {
        // GET
        public string Index()
        {
            return "This is my default action...";
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}