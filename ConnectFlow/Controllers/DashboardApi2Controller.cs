using ConnectFlow.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConnectFlow.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard2")]
    public class DashboardApi2Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardApi2Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/dashboard/integracoes
        [HttpGet("integracoes")]
        public async Task<IActionResult> GetIntegracoes()
        {
            var identificador = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(identificador, out int idUsuario))
            {
                return Unauthorized();
            }

            var configuracoes = await _context.Configuracoes
                .AsNoTracking()
                .Where(c => c.IdUsuario == idUsuario)
                .Include(c => c.Cliente)
                .Include(c => c.Logs)
                .ToListAsync();

            var integracoes = configuracoes.Select(c =>
            {
                var ultimoLog = c.Logs?
                    .OrderByDescending(log => log.DataHora)
                    .FirstOrDefault();

                var status = ultimoLog?.Status ?? "Sem registros";

                var statusNormalizado = status.ToLowerInvariant();

                bool online =
                    statusNormalizado.Contains("online") ||
                    statusNormalizado.Contains("sucesso");

                bool offline =
                    statusNormalizado.Contains("offline") ||
                    statusNormalizado.Contains("erro") ||
                    statusNormalizado.Contains("falha");

                return new
                {
                    c.IdConfig,
                    Cliente = c.Cliente?.RazaoSocial,
                    c.SistemaOrigem,
                    c.SistemaDestino,
                    Status = status,
                    Online = online,
                    Offline = offline,
                    UltimaAtualizacao = ultimoLog?.DataHora,
                    Mensagem = ultimoLog?.Mensagem
                };
            }).ToList();

            return Ok(new
            {
                TotalIntegracoes = integracoes.Count,
                IntegracoesOnline = integracoes.Count(i => i.Online),
                IntegracoesOffline = integracoes.Count(i => i.Offline),
                Integracoes = integracoes
            });
        }
    }
}