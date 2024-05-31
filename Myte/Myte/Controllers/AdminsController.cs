using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Myte.Data;
using Myte.Models;

namespace Myte.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchFuncionario, string searchWBS, DateTime? dataInicio, DateTime? dataFim)
        {
            var registros = _context.Registro
                .Include(r => r.Funcionario)
                .Include(r => r.WBS)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchFuncionario))
            {
                registros = registros.Where(r => r.Funcionario.FuncionarioNome.Contains(searchFuncionario));
            }

            if (!string.IsNullOrEmpty(searchWBS))
            {
                registros = registros.Where(r => r.WBS.Codigo.Contains(searchWBS));
            }

            // Filtra por dats
            if (dataInicio.HasValue && dataFim.HasValue)
            {
                registros = registros.Where(r =>
                    r.DataRegistro >= dataInicio.Value && r.DataRegistro <= dataFim.Value);
            }

            return View(registros.ToList());
        }


        // GET: Admins
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Registro.Include(r => r.Funcionario).Include(r => r.WBS);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.Registro
                .Include(r => r.Funcionario)
                .Include(r => r.WBS)
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (registro == null)
            {
                return NotFound();
            }

            return View(registro);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionario, "FuncionarioId", "FuncionarioNome");
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Codigo");
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistroId,FuncionarioId,WBSId,HorasTrab,DataRegistro")] Registro registro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registro);
                await _context.SaveChangesAsync();
                TempData["message"] = "ADMINS CRIADO COM SUCESSO";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionario, "FuncionarioId", "FuncionarioNome", registro.FuncionarioId);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Codigo", registro.WBSId);
            return View(registro);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.Registro.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionario, "FuncionarioId", "FuncionarioNome", registro.FuncionarioId);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Codigo", registro.WBSId);
            return View(registro);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistroId,FuncionarioId,WBSId,HorasTrab,DataRegistro")] Registro registro)
        {
            if (id != registro.RegistroId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registro);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "REGISTRO EDITADO COM SUCESSO";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistroExists(registro.RegistroId))
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
            ViewData["FuncionarioId"] = new SelectList(_context.Funcionario, "FuncionarioId", "FuncionarioNome", registro.FuncionarioId);
            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Codigo", registro.WBSId);
            return View(registro);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.Registro
                .Include(r => r.Funcionario)
                .Include(r => r.WBS)
                .FirstOrDefaultAsync(m => m.RegistroId == id);
            if (registro == null)
            {
                return NotFound();
            }

            return View(registro);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registro = await _context.Registro.FindAsync(id);
            if (registro != null)
            {
                _context.Registro.Remove(registro);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = "REGISTRO DELETADO COM SUCESSO";
            return RedirectToAction(nameof(Index));
        }

        private bool RegistroExists(int id)
        {
            return _context.Registro.Any(e => e.RegistroId == id);
        }
    }
}
