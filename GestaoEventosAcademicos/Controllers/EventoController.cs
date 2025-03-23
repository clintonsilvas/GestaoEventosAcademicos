using Microsoft.AspNetCore.Mvc;
using GestaoEventosAcademicos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GestaoEventosAcademicos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EventoController : Controller
    {
        public Context context;
        private readonly UserManager<Usuario> _userManager;

        public EventoController(Context ctx, UserManager<Usuario> userManager)
        {
            context = ctx;
            _userManager = userManager;
        }
        public IActionResult Index()
        {            
            var eventos = context.Eventos.Include(e => e.Administrador);
            return View(eventos);
        }
        public IActionResult Create()
        {
            var adminId = _userManager.GetUserId(User); 
            var admin = _userManager.Users.FirstOrDefault(u => u.Id == adminId); 
            ViewBag.AdministradorID = adminId; 
            ViewBag.AdministradorNome = admin.Nome;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Evento evento)
        {
            context.Add(evento);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var evento = context.Eventos.Include(e => e.Administrador).FirstOrDefault(e => e.EventoID == id);
            return View(evento);
        }

        public IActionResult Edit(int id)
        {
            var adminId = _userManager.GetUserId(User); // Obtém o ID do administrador logado 
            var admin = _userManager.Users.FirstOrDefault(u => u.Id == adminId);

            ViewBag.AdministradorID = adminId; // Passa o ID do administrador para a View
            ViewBag.AdministradorNome = admin.Nome; // Passa o nome do administrador para exibição
            var evento = context.Eventos.Find(id);
            return View(evento);
        }


        [HttpPost]
        public IActionResult Edit(Evento evento)
        {          
            context.Entry(evento).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var evento = context.Eventos.Include(f => f.Administrador).FirstOrDefault(p => p.EventoID == id);  
            return View(evento);
        }

        [HttpPost]
        public IActionResult Delete(Evento evento)
        {            
            context.Eventos.Remove(evento);
            context.SaveChanges();            
            return RedirectToAction("Index");
        }


        //buscar participantes do evento e cabeçalho com dados do evento
        public IActionResult Participantes(int id)
        {
            var evento = context.Eventos
                                .FirstOrDefault(e => e.EventoID == id);

            if (evento == null)
            {
                TempData["Erro"] = "Evento não encontrado.";
                return RedirectToAction("Index");
            }

            var inscritos = context.Inscricoes
                                   .Where(i => i.EventoID == id)
                                   .Include(i => i.Participante) // Inclui os dados do participante
                                   .Select(i => i.Participante) // Obtém apenas os participantes
                                   .ToList();

            ViewBag.Evento = evento; // Passa os detalhes do evento para a View
            return View(inscritos);
        }

    }
}
