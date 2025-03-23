using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestaoEventosAcademicos.Models
{
    public class Inscricao
    {
        public int InscricaoID { get; set; }
        public DateTime DataInscricao { get; set; } = DateTime.Now;


        public string ParticipanteID { get; set; }        
        public Participante Participante { get; set; }

        
        public int EventoID { get; set; }         
        public virtual Evento Evento { get; set; }

        public bool Concluido { get; set; }
        
    }
}

