using ConnectFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ConnectFlow.Data;

namespace ConnectFlow.Controllers
{
    public class HomeController : Controller
    {
        // 1. Variável para acessar o banco de dados
        private readonly ApplicationDbContext _context;

        // 2. Construtor que recebe o contexto do banco automaticamente
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CadastroUsuario()

        {
            return View();
        }
        [HttpPost]
        public IActionResult CadastroUsuario(Configuracao model)
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult MySQL()
        {
            return View();
        }

        public IActionResult Sap()
        {
            return View();
        }

        public IActionResult Apis()
        {
            return View();
        }

        public IActionResult Consultoria()
        {
            return View();
        }

        [Route("Creditos")]
        public IActionResult Creditos()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Acessibilidade(Configuracao model)
        {
            // Ajuste o caminho se a sua view estiver em outra pasta, como /Views/Home/
            return View("~/Views/Clientes/Acessibilidade.cshtml", model);
        }

        // 3. A nova função que recebe os dados da tela e salva no banco sem dar erro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarIntegracao(
            // Dados da Aba Empresa
            string cnpj, string razaoSocial, string nomeFantasia, string emailCorporativo, string telefone, string responsavelTecnico,
            // Dados da Aba Integração
            string mensagem,
            // Dados da Nova Aba de Acesso (Login)
            string nomeUsuario, string emailLogin, string senhaLogin)
        {
            //if (string.IsNullOrEmpty(cnpj) || string.IsNullOrEmpty(emailLogin)) return RedirectToAction("Index");

            // 1. Cadastra a Empresa (Cliente)
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Cnpj = cnpj,
                    RazaoSocial = razaoSocial,
                    NomeFantasia = nomeFantasia,
                    EmailCorporativo = emailCorporativo,
                    Telefone = telefone,
                    ResponsavelTecnico = responsavelTecnico,
                    DataCadastro = DateTime.Now
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }

            // 2. Cadastra o Utilizador (Login)
            // 2. Cadastra o Utilizador (Login)
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailLogin);
            if (usuario == null)
            {
                var contemEspecial = System.Text.RegularExpressions.Regex.IsMatch(senhaLogin, @"[!@#$%^&*(),.?:{}|<>]");

                if (!contemEspecial || senhaLogin.Length < 6)
                {
                    ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos 6 caracteres e incluir ao menos um caractere especial (!@#$%...).");
                    return View("~/Views/Clientes/CadastroUsuario.cshtml");
                }

                // CRUCIAL: Aplica a criptografia antes de salvar para o login funcionar depois!
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(senhaLogin);
                    var hashBytes = sha256.ComputeHash(bytes);
                    string senhaCriptografada = string.Concat(hashBytes.Select(b => b.ToString("x2")));

                    usuario = new Usuario
                    {
                        Nome = nomeUsuario,
                        Email = emailLogin,
                        SenhaHash = senhaCriptografada, // Salva o hash seguro!
                        Perfil = "Cliente",
                        DataCriacao = DateTime.Now
                    };
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
            }

            // 3. Regista o Chamado de Suporte
            var novoChamado = new ChamadoSuporte
            {
                IdCliente = cliente.IdCliente,
                NomeCompleto = nomeUsuario,
                EmailCorporativo = emailCorporativo,
                Mensagem = mensagem,
                DataEnvio = DateTime.Now
            };
            _context.ChamadosSuporte.Add(novoChamado);
            await _context.SaveChangesAsync();

            return View("~/Views/Home/Sucesso.cshtml");

        }
        public IActionResult Sucesso()
        {
            return View("~/Views/Clientes/Sucesso.cshtml");
        }
        [HttpPost]
        public async Task<IActionResult> Autenticar(string email, string senha)
        {
            // Criptografa a senha digitada para comparar com o banco
            string senhaCriptografada;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(senha);
                var hashBytes = sha256.ComputeHash(bytes);
                senhaCriptografada = string.Concat(hashBytes.Select(b => b.ToString("x2")));
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.SenhaHash == senhaCriptografada);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha incorretos. Verifique os seus dados.");
                return View("Login");
            }

            return RedirectToAction("PainelCliente", new { id = usuario.IdUsuario });
        }

        public async Task<IActionResult> PainelCliente(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var chamados = await _context.ChamadosSuporte
                .Where(c => c.EmailCorporativo == usuario.Email)
                .ToListAsync();

            ViewBag.Usuario = usuario;
            return View(chamados);
        }
    }
}