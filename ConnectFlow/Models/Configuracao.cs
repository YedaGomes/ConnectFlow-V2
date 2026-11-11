using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Models
{
    public class Configuracao
    {
        [Key]
        public int IdConfig { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; } 

        public string SistemaOrigem { get; set; } = string.Empty;

        public string SistemaDestino { get; set; } = string.Empty;

        public string VolumeDados { get; set; } = string.Empty;

        public string nomeUsuario { get; set; }
        public string emailLogin { get; set; }
        public string senhaLogin { get; set; }

        public bool SuporteLibras { get; set; }

        public bool NotificacoesEmail { get; set; }

        public int? IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<LogTransacao> Logs { get; set; } = new List<LogTransacao>();
    }
}
