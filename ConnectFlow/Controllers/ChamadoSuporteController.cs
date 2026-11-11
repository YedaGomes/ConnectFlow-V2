
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConnectFlow.Models;
using ConnectFlow.Data;

public class ChamadoSuporteController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChamadoSuporteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CHAMADOSUPORTES
    // GET: ChamadosSuporte
    public async Task<IActionResult> Index(string searchString)
    {
        var chamados = _context.ChamadosSuporte
            .Include(c => c.Cliente)
            .AsQueryable();

        // Filtra pelo nome completo ou pela mensagem enviada no chamado
        if (!string.IsNullOrEmpty(searchString))
        {
            chamados = chamados.Where(c => c.NomeCompleto.Contains(searchString) || c.Mensagem.Contains(searchString));
        }

        // Guarda o termo pesquisado para manter o campo preenchido na view
        ViewData["CurrentFilter"] = searchString;

        return View(await chamados.ToListAsync());
    }

    // GET: CHAMADOSUPORTES/Details/5
    public async Task<IActionResult> Details(int? idchamado)
    {
        if (idchamado == null)
        {
            return NotFound();
        }

        var chamadosuporte = await _context.ChamadosSuporte
            .FirstOrDefaultAsync(m => m.IdChamado == idchamado);
        if (chamadosuporte == null)
        {
            return NotFound();
        }

        return View(chamadosuporte);
    }

    // GET: CHAMADOSUPORTES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CHAMADOSUPORTES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdChamado,IdCliente,Cliente,NomeCompleto,EmailCorporativo,Mensagem,DataEnvio")] ChamadoSuporte chamadosuporte)
    {
        if (ModelState.IsValid)
        {
            _context.Add(chamadosuporte);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(chamadosuporte);
    }

    // GET: CHAMADOSUPORTES/Edit/5
    public async Task<IActionResult> Edit(int? idchamado)
    {
        if (idchamado == null)
        {
            return NotFound();
        }

        var chamadosuporte = await _context.ChamadosSuporte.FindAsync(idchamado);
        if (chamadosuporte == null)
        {
            return NotFound();
        }
        return View(chamadosuporte);
    }

    // POST: CHAMADOSUPORTES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? idchamado, [Bind("IdChamado,IdCliente,Cliente,NomeCompleto,EmailCorporativo,Mensagem,DataEnvio")] ChamadoSuporte chamadosuporte)
    {
        if (idchamado != chamadosuporte.IdChamado)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(chamadosuporte);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChamadoSuporteExists(chamadosuporte.IdChamado))
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
        return View(chamadosuporte);
    }

    // GET: CHAMADOSUPORTES/Delete/5
    public async Task<IActionResult> Delete(int? idchamado)
    {
        if (idchamado == null)
        {
            return NotFound();
        }

        var chamadosuporte = await _context.ChamadosSuporte
            .FirstOrDefaultAsync(m => m.IdChamado == idchamado);
        if (chamadosuporte == null)
        {
            return NotFound();
        }

        return View(chamadosuporte);
    }

    // POST: CHAMADOSUPORTES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idchamado)
    {
        var chamadosuporte = await _context.ChamadosSuporte.FindAsync(idchamado);
        if (chamadosuporte != null)
        {
            _context.ChamadosSuporte.Remove(chamadosuporte);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ChamadoSuporteExists(int? idchamado)
    {
        return _context.ChamadosSuporte.Any(e => e.IdChamado == idchamado);
    }
}
