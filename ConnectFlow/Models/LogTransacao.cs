using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Models
{
    public class LogTransacao
    {
        [Key]
        public int IdLog { get; set; }

        [Required]
        public int IdConfig { get; set; }

        [ForeignKey("IdConfig")]
        public virtual Configuracao Configuracao { get; set; }

        public DateTime DataHora { get; set; }

        public string Status { get; set; }

        public string Mensagem { get; set; }
    }
}