using ConnectFlow.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectFlow.Controllers.Api
{
    // Mude a rota para ficar fixa em "api/dashboard"
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        // ... (o resto do código fica igual)
        private readonly ApplicationDbContext _context;

        public DashboardApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 3. Define que é um método GET e espera um ID na URL
        [HttpGet("{idCliente}")]
        public async Task<IActionResult> GetStatusIntegracoes(int idCliente)
        {
            // Busca o cliente no banco
            var cliente = await _context.Clientes.FindAsync(idCliente);

            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            // Conta quantos chamados esse cliente tem aberto (exemplo prático)
            var totalChamados = await _context.ChamadosSuporte
                .Where(c => c.IdCliente == idCliente)
                .CountAsync();

            // Monta o objeto que será transformado em JSON
            var resultado = new
            {
                Cliente = cliente.RazaoSocial,
                Cnpj = cliente.Cnpj,
                Resumo = new
                {
                    StatusSistema = "Online",
                    ChamadosAbertos = totalChamados,
                    Mensagem = "Sistemas operando normalmente."
                }
            };

            // 4. Retorna status 200 (OK) com o JSON
            return Ok(resultado);
        }
    }
}