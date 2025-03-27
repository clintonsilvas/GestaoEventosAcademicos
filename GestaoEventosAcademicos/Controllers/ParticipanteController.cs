using DinkToPdf;
using GestaoEventosAcademicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace GestaoEventosAcademicos.Controllers
{
    [Authorize(Roles = "Participante")]
    public class ParticipanteController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<Usuario> _userManager;

        public ParticipanteController(Context context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var eventos = _context.Eventos.ToList();
            return View(eventos);
        }
        public IActionResult ConfirmarInscricao(int eventoId)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.EventoID == eventoId);
            return View(evento);
        }

        [HttpPost]
        public IActionResult Inscrever(int eventoId)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.EventoID == eventoId);
            var userId = _userManager.GetUserId(User);
            bool jaInscrito = _context.Inscricoes.Any(i => i.ParticipanteID == userId && i.EventoID == eventoId);

            if (!jaInscrito)
            {
                var inscricao = new Inscricao
                {
                    ParticipanteID = userId,
                    EventoID = eventoId,
                    DataInscricao = DateTime.Now
                };

                _context.Inscricoes.Add(inscricao);
                _context.SaveChanges();

                TempData["Message"] = "Inscrição realizada com sucesso! Veja detalhes em Seus eventos";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "Você já está inscrito neste evento.";
                return RedirectToAction("Index");
            }
        }
        public IActionResult MeusEventos()
        {
            var userId = _userManager.GetUserId(User);

            // Obtendo os eventos em que o usuário está inscrito
            var eventosInscritos = _context.Inscricoes
                .Where(i => i.ParticipanteID == userId)
                .Include(i => i.Evento)
                .Select(i => i.Evento)
                .ToList();

            // Obtendo os IDs dos eventos em que o usuário já recebeu certificado
            var eventosComCertificado = _context.Certificados
                .Where(c => c.ParticipanteID == userId)
                .Select(c => c.EventoID)
                .ToList();

            // Passando a lista para a ViewBag
            ViewBag.EventosComCertificado = eventosComCertificado;

            return View(eventosInscritos);
        }

        public IActionResult ConfirmarCancelarInscricao(int eventoId)
        {
            var evento = _context.Eventos.FirstOrDefault(e => e.EventoID == eventoId);
            return View(evento);
        }

        [HttpPost]
        public IActionResult CancelarInscricao(int eventoId)
        {
            var userId = _userManager.GetUserId(User);
            var inscricao = _context.Inscricoes
                .FirstOrDefault(i => i.ParticipanteID == userId && i.EventoID == eventoId);

            if (inscricao != null)
            {
                _context.Inscricoes.Remove(inscricao);
                _context.SaveChanges();
            }
            return RedirectToAction("MeusEventos");
        }

        public IActionResult Perfil()
        {
            var participanteID = _userManager.GetUserId(User);
            var participante = _context.Participantes
                                       .Include(p => p.Curso)
                                       .FirstOrDefault(p => p.Id == participanteID);

            return View(participante);
        }

        public IActionResult Certificados()
        {
            var userId = _userManager.GetUserId(User);  // Obtém o ID do usuário logado
            var certificados = _context.Certificados
                .Where(c => c.ParticipanteID == userId)
                .Include(c => c.Evento)
                .Include(c => c.Participante) // Inclui as informações do participante
                .ThenInclude(p => p.Curso)    // Inclui o curso do participante
                .ToList();

            return View(certificados);
        }
        public IActionResult VisualizarCertificado(int id)
        {
            var certificado = _context.Certificados
                .Include(c => c.Evento)
                .ThenInclude(e => e.Administrador) // Inclui Administrador do Evento
                .Include(c => c.Participante)
                .ThenInclude(p => p.Curso) // Inclui o Curso do Participante
                .FirstOrDefault(c => c.CertificadoID == id);

            if (certificado == null)
            {
                return NotFound("Certificado não encontrado.");
            }

            return View(certificado);
        }

    }
}
