using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dikubot.Pages
{
    public class _HostModel : PageModel
    {
        
        [FromQuery(Name = "code")]
        public string AuthCode { get; set; }

        public void OnGet()
        {
           
        }
    }
}

