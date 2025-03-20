namespace GestaoEventosAcademicos.Models
{
    public class Certificado
    {
        public int CertificadoID {  get; set; }
        public string Descricao { get; set; }
        public DateTime DataEmissao { get; set; }


        public int ParticipanteID { get; set; }
        public Participante Participante { get; set; }

        public int EventoID {  get; set; }
        public Evento Evento { get; set; }

        public int AdministradorID { get; set; }
        public Administrador Administrador { get; set; }
    }
}
