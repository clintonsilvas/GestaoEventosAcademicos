namespace GestaoEventosAcademicos.Models
{
    public class Administrador : Usuario
    {
        public ICollection<Evento> EventosAdministrados { get; set; }
        public ICollection<Certificado> Certificados { get; set; }
    }
}
