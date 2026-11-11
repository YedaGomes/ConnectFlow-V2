using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Key]
        [Column("ID")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$", ErrorMessage = "Formato inválido. Use 00.000.000/0000-00")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "O CNPJ completo deve ter 18 caracteres")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Razão Social é obrigatória")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A Razão Social deve ter entre 3 e 100 caracteres")]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome Fantasia é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O Nome Fantasia deve ter entre 3 e 100 caracteres")]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Digite um e-mail válido (ex: contato@empresa.com)")]
        public string EmailCorporativo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Telefone é obrigatório")]
        [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$", ErrorMessage = "Formato inválido. Use (99) 99999-9999")]
        [StringLength(15, MinimumLength = 14, ErrorMessage = "O Telefone deve estar no formato correto")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Responsável Técnico é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome do responsável deve ter no mínimo 3 caracteres")]
        public string ResponsavelTecnico { get; set; } = string.Empty;

        public string SistemasOrigem { get; set; } = string.Empty;
        public string SistemasDestino { get; set; } = string.Empty;
        public string VolumeDadosMensal { get; set; } = string.Empty;

        public bool SuporteLibras { get; set; }
        public bool NotificacoesEmail { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}