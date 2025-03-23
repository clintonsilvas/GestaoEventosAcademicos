using System.ComponentModel.DataAnnotations;

namespace GestaoEventosAcademicos.Models
{
    public class Participante : Usuario
    {
        [Required(ErrorMessage = "O curso é obrigatório.")]
        public int CursoID { get; set; }
        public Curso Curso { get; set; }
    }
}
