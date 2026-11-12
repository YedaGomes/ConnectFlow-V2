using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConnectFlow.Data;
using ConnectFlow.Models;

namespace ConnectFlow.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }
        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCliente,Cnpj,RazaoSocial,NomeFantasia,ResponsavelTecnico,EmailCorporativo,Telefone")] Cliente cliente)
        {
            // 1. Trava: Verifica se o CNPJ já existe no banco
            bool cnpjExiste = await _context.Clientes.AnyAsync(c => c.Cnpj == cliente.Cnpj);
            if (cnpjExiste)
            {
                ModelState.AddModelError("Cnpj", "Este CNPJ já está cadastrado no sistema.");
            }

            // 2. Trava: Verifica se o E-mail Corporativo já existe no banco
            bool emailExiste = await _context.Clientes.AnyAsync(c => c.EmailCorporativo == cliente.EmailCorporativo);
            if (emailExiste)
            {
                ModelState.AddModelError("EmailCorporativo", "Este e-mail corporativo já está em uso.");
            }

            // 3. Só prossegue se não houver duplicidade ou campos vazios
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Configuracao", new { id = cliente.IdCliente });
            }

            // Se der erro, devolve a tela preenchida com o aviso em vermelho
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCliente,Cnpj,RazaoSocial,NomeFantasia,ResponsavelTecnico,EmailCorporativo,Telefone")] Cliente cliente)
        {
            if (id != cliente.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.IdCliente))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.IdCliente == id);
        }
    
         public IActionResult Configuracao(int id)
        {
            var config = new ConnectFlow.Models.Configuracao { IdCliente = id };
            return View(config);
        }
        // ETAPA 2: Salva a Configuração no banco com validação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configuracao(int id, string prioridade, string bancoOrigem)
        {
            // 1. TRAVA: Se não selecionou a prioridade ou o banco, bloqueia e avisa
            if (string.IsNullOrEmpty(prioridade) || string.IsNullOrEmpty(bancoOrigem))
            {
                TempData["ErroConfig"] = "Você precisa selecionar uma prioridade e o banco de dados de origem.";
                var configErro = new ConnectFlow.Models.Configuracao { IdCliente = id };
                return View(configErro);
            }

            // 2. Cria a configuração no banco de dados vinculada ao Cliente (id)
            var config = new ConnectFlow.Models.Configuracao
            {
                IdCliente = id,
                VolumeDados = prioridade, // Armazena a prioridade escolhida
                SistemaOrigem = bancoOrigem, // Armazena o banco escolhido
                SistemaDestino = "Pendente"
            };

            _context.Configuracoes.Add(config);
            await _context.SaveChangesAsync();

            // 3. Redireciona para a Etapa 3 (Usuário), passando o ID do Cliente
            return RedirectToAction("CadastroUsuario", "Home", new { id = id });
        }

        [HttpGet]
        public IActionResult Acessibilidade(int id)
        {
            var config = new ConnectFlow.Models.Configuracao { IdCliente = id };
            return View("Acessibilidade", config);
        }

        // ETAPA 3: Salva a Acessibilidade na mesma configuração criada na Etapa 2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Acessibilidade(int id, bool suporteLibras, bool notificacoesEmail)
        {
            // 1. Busca no banco a configuração que criamos na etapa anterior
            var config = await _context.Configuracoes.FirstOrDefaultAsync(c => c.IdCliente == id);

            if (config != null)
            {
                // 2. Atualiza os dados com as checkboxes marcadas na tela
                config.SuporteLibras = suporteLibras;
                config.NotificacoesEmail = notificacoesEmail;

                _context.Update(config);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Sucesso");
        }

    }

}
