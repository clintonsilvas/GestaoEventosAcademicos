using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosAcademicos.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
