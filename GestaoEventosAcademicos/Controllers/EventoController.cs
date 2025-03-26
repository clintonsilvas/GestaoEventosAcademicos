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
            var evento = context.Eventos.FirstOrDefault(e => e.EventoID == id);

            if (evento == null)
            {
                TempData["Erro"] = "Evento não encontrado.";
                return RedirectToAction("Index");
            }

            var inscritos = context.Inscricoes
                .Where(i => i.EventoID == id)
                .Include(i => i.Participante)
                .Select(i => i.Participante)
                .ToList();

            // Pegando os IDs dos participantes que já possuem certificado
            var participantesComCertificado = context.Certificados
                .Where(c => c.EventoID == id)
                .Select(c => c.ParticipanteID)
                .ToList();

            ViewBag.ParticipantesComCertificado = participantesComCertificado;

            ViewBag.Evento = evento; // Passando os dados do evento
            return View(inscritos);
        }


        [HttpPost]
        public IActionResult AtualizarPresenca(List<string> participantesPresentes, int eventoId)
        {
            if (participantesPresentes != null && participantesPresentes.Any())
            {
                // Busca as inscrições para o evento específico e marca os participantes como concluídos
                var inscricoes = context.Inscricoes
                    .Where(i => i.EventoID == eventoId && participantesPresentes.Contains(i.ParticipanteID))
                    .ToList();

                foreach (var inscricao in inscricoes)
                {
                    // Marca como concluído (presença confirmada)
                    inscricao.Concluido = true;

                    // Gera o certificado para o participante, se ainda não houver um
                    var certificadoExistente = context.Certificados
                        .FirstOrDefault(c => c.ParticipanteID == inscricao.ParticipanteID && c.EventoID == eventoId);

                    if (certificadoExistente == null)
                    {
                        // Obtém o participante do banco de dados para acessar o curso
                        var participante = context.Participantes
                            .Include(p => p.Curso) // Inclui a informação do curso do participante
                            .FirstOrDefault(p => p.Id == inscricao.ParticipanteID);

                        // Gera a descrição do certificado com o nome do curso
                        string descricaoCertificado = $"Certificado de Participação do Evento - Curso: {participante?.Curso?.Nome}";

                        // Cria o certificado
                        var certificado = new Certificado
                        {
                            ParticipanteID = inscricao.ParticipanteID,
                            EventoID = eventoId,
                            Descricao = descricaoCertificado,  // Descrição do certificado com o nome do curso
                            DataEmissao = DateTime.Now,
                            AdministradorID = _userManager.GetUserId(User) // ID do administrador logado
                        };

                        // Adiciona o certificado à base de dados
                        context.Certificados.Add(certificado);
                    }
                }

                context.SaveChanges();
                TempData["Sucesso"] = "Presença confirmada e certificados emitidos com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Nenhum participante selecionado.";
            }

            // Redireciona para a página de participantes
            return RedirectToAction("Participantes", new { id = eventoId });
        }


    }
}
