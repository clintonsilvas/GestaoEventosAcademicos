namespace GestaoEventosAcademicos.Models
{
    public class Participante
    {
        public int ParticipanteID { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Email {  get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public bool Certificado { get; set; }

        public int CursoID { get; set; }
        public Curso Curso { get; set; }

        public int? EventoID { get; set; }
        public Evento Evento { get; set; }

    }
}
