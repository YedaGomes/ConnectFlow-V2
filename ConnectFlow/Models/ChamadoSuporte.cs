using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Models
{
    [Table("ChamadoSuporte")]
    public class ChamadoSuporte
    {
        [Key]
        public int IdChamado { get; set; }

        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeCompleto { get; set; }

        [Required]
        [MaxLength(100)]
        public string EmailCorporativo { get; set; }

        [Required]
        public string Mensagem { get; set; }

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Aberto";
    }
}