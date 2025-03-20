using Microsoft.AspNetCore.Mvc;
using GestaoEventosAcademicos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestaoEventosAcademicos.Controllers
{
    public class EventoController : Controller
    {
        public Context context;

        public EventoController(Context ctx)
        {
            context = ctx;
        }
        public IActionResult Index()
        {
            var eventos = context.Eventos.Include(e => e.Administrador);
            return View(eventos);
        }

        public IActionResult Create()
        {
            ViewBag.Administrador = new SelectList(context.Administradores.OrderBy(f => f.Nome), "AdministradorID", "Nome");
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
            var evento = context.Eventos
                                .Include(e => e.Administrador)
                                .FirstOrDefault(e => e.EventoID == id);

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        public IActionResult Edit(int id)
        {
            var evento = context.Eventos.Find(id);

            if (evento == null)
            {
                return NotFound();
            }

            ViewBag.Administrador = new SelectList(context.Administradores.OrderBy(f => f.Nome), "AdministradorID", "Nome", evento.AdministradorID);
            return View(evento);
        }

        [HttpPost]
        public IActionResult Edit(Evento evento)
        {
            //avisa a EF que o registro será modificado
            context.Entry(evento).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var evento = context.Eventos.Include(f => f.Administrador).FirstOrDefault(p => p.EventoID == id);            
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        [HttpPost]
        public IActionResult Delete(Evento evento)
        {            
            context.Eventos.Remove(evento);
            context.SaveChanges();            
            return RedirectToAction("Index");
        }

        public IActionResult Participantes(int id)
        {
            var evento = context.Eventos
                                .Include(e => e.Administrador)
                                .Include(e => e.Participantes)
                                .FirstOrDefault(e => e.EventoID == id);

            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        public IActionResult InscreverParticipante(int id)
        {
            var evento = context.Eventos
                                .Include(e => e.Administrador)
                                .Include(e => e.Participantes)
                                .FirstOrDefault(e => e.EventoID == id);

            if (evento == null)
            {
                return NotFound();
            }

            // Filtra participantes ainda não inscritos
            var participantesDisponiveis = context.Participantes
                                                  .Where(p => !evento.Participantes.Contains(p))
                                                  .OrderBy(p => p.Nome)
                                                  .ToList();

            ViewBag.Participantes = participantesDisponiveis;
            return View(evento);
        }


        [HttpPost]
        public IActionResult InscreverParticipante(int id, int participanteId)
        {
            var evento = context.Eventos.Include(e => e.Participantes).FirstOrDefault(e => e.EventoID == id);
            var participante = context.Participantes.FirstOrDefault(p => p.ParticipanteID == participanteId);

            if (evento == null || participante == null)
            {
                return NotFound();
            }

            // Verifica se o participante já está inscrito
            if (evento.Participantes.Any(p => p.ParticipanteID == participanteId))
            {
                TempData["ErrorMessage"] = "Participante já está inscrito neste evento.";
                return RedirectToAction("InscreverParticipante", new { id });
            }

            // Adiciona o participante ao evento
            evento.Participantes.Add(participante);
            context.SaveChanges();

            TempData["SuccessMessage"] = "Inscrição realizada com sucesso!";
            return RedirectToAction("Index");
        }



    }
}
