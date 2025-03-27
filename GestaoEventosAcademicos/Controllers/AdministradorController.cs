using GestaoEventosAcademicos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosAcademicos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradorController : Controller
    {
        public Context context;
        private readonly UserManager<Usuario> _userManager;
        public AdministradorController(Context ctx, UserManager<Usuario> userManager)
        {
            context = ctx;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View(context.Administradores);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Administrador administrador, string Senha)
        {
            administrador.UserName = administrador.Email;
            administrador.TipoUsuario = "Administrador";

            var resultado = _userManager.CreateAsync(administrador, Senha).Result;

            if (resultado.Succeeded)
            {
                // Adiciona o participante à role "Participante"
                var roleResult = _userManager.AddToRoleAsync(administrador, "Administrador").Result;

                if (roleResult.Succeeded)
                {
                    TempData["Sucesso"] = "Administrador cadastrado com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Administrador criado, mas não foi possível atribuir a role.";
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Sucesso"] = "Não foi possivel cadastrar o administrador";
                return RedirectToAction("Index");
            }
        }
        public IActionResult Details(int id)
        {
            var administrador = context.Administradores.FirstOrDefault();
            return View(administrador);
        }
        public IActionResult EventosAdministrados(string id)
        {
            var eventos = context.Eventos
                                .Include(e => e.Participantes)
                                .Where(e => e.AdministradorID == id) // Buscar eventos do Administrador
                                .ToList();

            return View(eventos);
        }

        public IActionResult Delete(string id)
        {
            var administrador = context.Administradores.FirstOrDefault(f => f.Id == id);
            return View(administrador);
        }
        [HttpPost]
        public IActionResult DeleteAdm(string id)
        {
            try
            {
                var administrador = _userManager.FindByIdAsync(id).Result;

                var resultado = _userManager.DeleteAsync(administrador).Result;

                if (resultado.Succeeded)
                {
                    TempData["Sucesso"] = "Administrador excluído com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao excluir administrador: " +
                        string.Join(", ", resultado.Errors.Select(e => e.Description));
                }
            }
            catch (Exception)
            {
                TempData["Erro"] = "Não foi possível excluir o administrador. Ele pode estar associado a eventos ou outras informações.";
            }

            return RedirectToAction("Index");
        }

        //VISUALIZAR TODOS OS PARTICIPANTES CADASTRADOOS
        public IActionResult ListarParticipantes()
        {
            var participantes = context.Participantes
                .Where(u => u.TipoUsuario == "Participante")
                .OrderBy(u => u.Nome)
                .ToList();

            return View(participantes);
        }

        public IActionResult CreateParticipante()
        {
            ViewBag.CursoID = new SelectList(context.Cursos.OrderBy(c => c.Nome), "CursoID", "Nome");
            return View();
        }


        [HttpPost]
        public IActionResult CreateParticipante(Participante participante, string senha)
        {
            participante.UserName = participante.Email;
            participante.TipoUsuario = "Participante";

            var resultado = _userManager.CreateAsync(participante, senha).Result;

            if (resultado.Succeeded)
            {
                // Adiciona o participante à role "Participante"
                var roleResult = _userManager.AddToRoleAsync(participante, "Participante").Result;

                if (roleResult.Succeeded)
                {
                    TempData["Sucesso"] = "Participante cadastrado com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Participante criado, mas não foi possível atribuir a role.";
                }

                return RedirectToAction("ListarParticipantes");
            }
            else
            {
                TempData["Erro"] = "Não foi possível cadastrar o Participante.";
                return RedirectToAction("ListarParticipantes");
            }
        }

        public IActionResult EditParticipante(string id)
        {
            var participante = _userManager.FindByIdAsync(id).Result;
            ViewBag.CursoID = new SelectList(context.Cursos, "CursoID", "Nome");
            return View(participante);
        }
        [HttpPost]
        public IActionResult EditParticipante(Participante model, string senha)
        {
            var participante = _userManager.Users
                                .OfType<Participante>()
                                .FirstOrDefault(p => p.Id == model.Id);


            if (participante != null)
            {
                participante.Nome = model.Nome;
                participante.Email = model.Email;
                participante.UserName = model.Email;
                participante.Telefone = model.Telefone;
                participante.DataNascimento = model.DataNascimento;
                participante.CursoID = model.CursoID;

                _userManager.UpdateAsync(participante).Wait();

                if (!string.IsNullOrEmpty(senha))
                {
                    var token = _userManager.GeneratePasswordResetTokenAsync(participante).Result;
                    _userManager.ResetPasswordAsync(participante, token, senha).Wait();
                }

                TempData["Sucesso"] = "Participante editado com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Erro ao editar participante.";
            }

            return RedirectToAction("ListarParticipantes");
        }

        public IActionResult DetailsParticipante(string id)
        {
            var participante = context.Participantes
                                      .Include(p => p.Curso)
                                      .FirstOrDefault(p => p.Id == id);

            if (participante == null)
            {
                TempData["Erro"] = "Participante não encontrado.";
                return RedirectToAction("ListarParticipantes");
            }

            return View(participante);
        }


        public IActionResult DeleteParticipante(string id)
        {
            var participante = context.Participantes
                                      .Include(p => p.Curso)
                                      .FirstOrDefault(p => p.Id == id);

            if (participante == null)
            {
                TempData["Erro"] = "Participante não encontrado.";
                return RedirectToAction("ListarParticipantes");
            }

            return View(participante);
        }

        // Excluir um participante
        [HttpPost]
        public IActionResult DeleteParticipanteConfirmed(string id)
        {
            var participante = context.Participantes.FirstOrDefault(p => p.Id == id);

            if (participante == null)
            {
                TempData["Erro"] = "Participante não encontrado.";
                return RedirectToAction("ListarParticipantes");
            }

            try
            {
                var resultado = _userManager.DeleteAsync(participante).Result;

                if (resultado.Succeeded)
                {
                    TempData["Sucesso"] = "Participante excluído com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Erro ao excluir participante: " + string.Join(", ", resultado.Errors.Select(e => e.Description));
                }
            }
            catch (Exception)
            {
                TempData["Erro"] = "Não foi possível excluir o participante. Ele pode estar associado a eventos.";
            }

            return RedirectToAction("ListarParticipantes");
        }

        public IActionResult Perfil()
        {
            var adminId = _userManager.GetUserId(User);
            var administrador = context.Administradores.FirstOrDefault(a => a.Id == adminId);
            return View(administrador);
        }
        public IActionResult EditarPerfil()
        {
            var adminId = _userManager.GetUserId(User);
            var administrador = context.Administradores.FirstOrDefault(a => a.Id == adminId);

            if (administrador == null)
            {
                TempData["Erro"] = "Administrador não encontrado.";
                return RedirectToAction("Perfil");
            }

            return View(administrador);
        }

        [HttpPost]
        public IActionResult EditarPerfil(Administrador administrador, string novaSenha)
        {
            var adminExistente = _userManager.FindByIdAsync(administrador.Id).Result;

            adminExistente.Nome = administrador.Nome;
            adminExistente.Email = administrador.Email;
            adminExistente.Telefone = administrador.Telefone;
            adminExistente.UserName = administrador.Email;

            var resultadoAtualizacao = _userManager.UpdateAsync(adminExistente).Result;

            if (!string.IsNullOrEmpty(novaSenha))
            {
                var token = _userManager.GeneratePasswordResetTokenAsync(adminExistente).Result;
                var resultadoSenha = _userManager.ResetPasswordAsync(adminExistente, token, novaSenha).Result;

            }

            if (resultadoAtualizacao.Succeeded)
            {
                TempData["Sucesso"] = "Perfil atualizado com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Erro ao atualizar perfil.";
            }

            return RedirectToAction("Perfil");
        }
    }
}
