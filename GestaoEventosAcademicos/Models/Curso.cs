namespace GestaoEventosAcademicos.Models
{
    public class Curso
    {
        public int CursoID { get; set; }
        public string Nome { get; set;}

        public ICollection<Participante> Participantes { get; set; }

    }
}
