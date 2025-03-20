namespace GestaoEventosAcademicos.Models
{
    public class Evento
    {
        public int EventoID {  get; set; }
        public string Nome { get; set; }
        public string Local {  get; set; }
        public DateTime Data { get; set; }
        public string Duração {  get; set; }
        public string CodigoParticipacao { get; set; }
        public int Capacidade { get; set; }

        public int AdministradorID {  get; set; }
        public Administrador Administrador { get; set; }

        public ICollection<Participante> Participantes { get; set; }    
        
        public ICollection<Certificado> Certificados { get; set;}
    }
}
