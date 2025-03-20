namespace GestaoEventosAcademicos.Models
{
    public class Administrador
    {
        public int AdministradorID { get; set; }
        public string Nome { get; set; }        
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }

        public ICollection<Evento> EventosAdministrados { get; set; }
        public ICollection<Certificado> Certificados { get; set; }
    }
}
