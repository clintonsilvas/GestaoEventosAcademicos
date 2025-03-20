using GestaoEventosAcademicos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosAcademicos.Controllers
{
    public class ParticipanteController : Controller
    {
        public Context context;

        public ParticipanteController(Context ctx)
        {
            context = ctx;
        }
        public IActionResult Index()
        {
            var participante = context.Participantes.Include(c => c.Curso).Include(c => c.Evento);
            return View(participante);
        }
        public IActionResult Create(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            CarregarViewBags();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Participante participante, string returnUrl = null)
        {
            context.Add(participante);
            context.SaveChanges();
            TempData["SuccessMessage"] = "Participante criado com sucesso!";

            // Se houver um returnUrl, redireciona para lá, senão, segue o fluxo normal
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            var participante = context.Participantes
                                .Include(p => p.Curso)
                                .Include(p => p.Evento)
                                .FirstOrDefault(p => p.ParticipanteID == id);

            if (participante == null)
            {
                return NotFound();
            }

            return View(participante);
        }

        public IActionResult Edit(int id)
        {
            var participante = context.Participantes.Find(id);                
            if (participante == null)
            {
                return NotFound();
            }

            CarregarViewBags(); 
            return View(participante);
        }


        [HttpPost]
        public IActionResult Edit(Participante participante)
        {
            //avisa a EF que o registro será modificado
            context.Entry(participante).State = EntityState.Modified;
            context.SaveChanges();
            TempData["SuccessMessage"] = "Participante editado com sucesso!";
            return RedirectToAction("Index");
        }
        private void CarregarViewBags()
        {
            ViewBag.Cursos = new SelectList(context.Cursos.OrderBy(f => f.Nome), "CursoID", "Nome");
            ViewBag.Eventos = new SelectList(context.Eventos.OrderBy(e => e.Nome), "EventoID", "Nome");
        }

        public IActionResult Delete(int id)
        {
            var participante = context.Participantes
                .Include(p => p.Curso)
                .Include(p => p.Evento)
                .FirstOrDefault(p => p.ParticipanteID == id);

            if (participante == null)
            {
                return NotFound();
            }

            return View(participante);
        }

        [HttpPost]
        public IActionResult Delete(Participante participante)
        {
            context.Participantes.Remove(participante);
            context.SaveChanges();
            TempData["SuccessMessage"] = "Participante excluido com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
