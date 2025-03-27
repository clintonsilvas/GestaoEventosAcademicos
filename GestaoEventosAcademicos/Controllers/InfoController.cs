using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosAcademicos.Controllers
{
    public class InfoController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        
        [AllowAnonymous]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}
