using GestaoEventosAcademicos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEventosAcademicos.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;

        public LoginController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string senha)
        {
            var user = _userManager.FindByEmailAsync(email).Result;

            if (user != null)
            {
                var resultado = _signInManager.PasswordSignInAsync(user, senha, false, false).Result;

                if (resultado.Succeeded)
                {
                    if (user.TipoUsuario == "Administrador")
                    {
                        return RedirectToAction("Index", "Evento");
                    }
                    else // Se for Participante ou outro tipo
                    {
                        return RedirectToAction("Index", "Participante");
                    }
                }
            }

            TempData["Erro"] = "Usuário ou senha inválidos.";
            return View();
        }
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
        
        public IActionResult RedefinirSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RedefinirSenha(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Erro"] = "Informe um e-mail válido.";
                return View();
            }

            // Simulação de envio de e-mail (substituir por lógica real)
            TempData["Sucesso"] = "Um link de redefinição de senha foi enviado para seu e-mail.";

            return RedirectToAction("Login");
        }

    }
}
