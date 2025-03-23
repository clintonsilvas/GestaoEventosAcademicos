using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace GestaoEventosAcademicos.Models
{
    public class Usuario: IdentityUser
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        public string TipoUsuario { get; set; } // "Administrador" ou "Participante"
    }
}
