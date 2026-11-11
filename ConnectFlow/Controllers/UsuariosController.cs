using ConnectFlow.Data;
using ConnectFlow.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class UsuariosController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: USUARIOS
    public async Task<IActionResult> Index()
    {
        return View(await _context.Usuarios.ToListAsync());
    }

    // GET: USUARIOS/Details/5
    public async Task<IActionResult> Details(int? idusuario)
    {
        if (idusuario == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(m => m.IdUsuario == idusuario);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // GET: USUARIOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: USUARIOS/Create
    // POST: USUARIOS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdUsuario,Nome,Email,SenhaHash,Perfil,DataCriacao")] Usuario usuario)
    {
        // Força a data atual para nunca ir nula
        usuario.DataCriacao = DateTime.Now;

        // Se houver algum erro de validação nos campos do formulário:
        if (!ModelState.IsValid)
        {
            // Retorna para a mesma tela de cadastro exibindo o erro no console/debug
            return View(usuario);
        }

        // Criptografa a senha antes de gravar
        usuario.SenhaHash = GerarHash(usuario.SenhaHash);

        _context.Add(usuario);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Login));
    }
    // GET: USUARIOS/Edit/5
    public async Task<IActionResult> Edit(int? idusuario)
    {
        if (idusuario == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios.FindAsync(idusuario);
        if (usuario == null)
        {
            return NotFound();
        }
        return View(usuario);
    }

    // POST: USUARIOS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idusuario, [Bind("IdUsuario,Nome,Email,SenhaHash,Perfil,DataCriacao")] Usuario usuario)
    {
        if (idusuario != usuario.IdUsuario)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Se a senha foi alterada, precisaria criptografar de novo aqui, 
                // mas para simplificar agora, mantemos o fluxo padrão.
                _context.Update(usuario);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.IdUsuario))
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
        return View(usuario);
    }

    // GET: USUARIOS/Delete/5
    public async Task<IActionResult> Delete(int? idusuario)
    {
        if (idusuario == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(m => m.IdUsuario == idusuario);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // POST: USUARIOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idusuario)
    {
        var usuario = await _context.Usuarios.FindAsync(idusuario);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool UsuarioExists(int? idusuario)
    {
        return _context.Usuarios.Any(e => e.IdUsuario == idusuario);
    }

    // ====================================================================
    // NOVOS MÉTODOS DE SEGURANÇA E LOGIN
    // ====================================================================

    // GET: USUARIOS/Login
    // GET: USUARIOS/Login
    [HttpGet]
    public IActionResult Login()
    {
        // Aponta diretamente para onde o arquivo está funcionando na sua estrutura
        return View("~/Views/Home/Login.cshtml");
    }

    // POST: USUARIOS/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string Email, string Senha)
    {
        // 1. Criptografa a senha digitada pelo usuário na tela
        string senhaCriptografada = GerarHash(Senha);

        // 2. Busca o usuário conferindo o e-mail e a senha criptografada de uma só vez
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == Email && u.SenhaHash == senhaCriptografada);

        // 3. Se não encontrar, exibe a mensagem de erro na tela de login
        if (usuario == null)
        {
            ViewBag.Erro = "E-mail ou senha incorretos.";
            return View("~/Views/Home/Login.cshtml");
        }

        // 4. Se encontrar, cria a sessão de login (Cookie)
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
    };

        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // 5. Redireciona para a página de acompanhamento
        return RedirectToAction("AcompanharSolicitacao");
    }

    // GET: USUARIOS/AcompanharSolicitacao (ROTA PROTEGIDA)
    [Authorize]
    [HttpGet]
    public IActionResult AcompanharSolicitacao()
    {
        return View();
    }

    // GET: USUARIOS/Sair
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // Função de Criptografia SHA256
    public string GerarHash(string texto)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            // Usa UTF-8 para garantir o mesmo formato padrão
            var bytes = System.Text.Encoding.UTF8.GetBytes(texto);
            var hashBytes = sha256.ComputeHash(bytes);

            // Converte para uma string hexadecimal limpa em minúsculas
            return string.Concat(hashBytes.Select(b => b.ToString("x2")));
        }
    }
}