using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Myte.Data;
using Myte.Models;

namespace Myte.Controllers
{
    public class RegistroesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistroesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdmIndex()
        {
            return View();
        }

        // GET: Registroes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Registro.Include(r => r.Funcionario).Include(r => r.WBS);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Registroes/Details/5
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

        // GET: Registroes/Create
        [HttpGet]

        public IActionResult Create()
        {
            ViewData["FuncionarioId"] = new SelectList(_context.Set<Funcionario>(), "FuncionarioId", "FuncionarioNome");
            var wbsList = _context.WBS.Select(w => new { w.WBSId, w.Codigo }).ToList();
            ViewData["WBSList"] = new SelectList(wbsList, "WBSId", "Codigo");
            return View();
        }





        // POST: Registroes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistroId,FuncionarioId,WBSId,HorasTrab,DataRegistro")] Registro registro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registro);
                await _context.SaveChangesAsync();
                TempData["message"] = "REGISTRO CRIADO COM SUCESSO";
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncionarioId"] = new SelectList(_context.Set<Funcionario>(), "FuncionarioId", "FuncionarioNome", registro.FuncionarioId);
            ViewData["WBSList"] = _context.WBS.ToList();
            return View(registro);
        }

        // GET: Registroes/Edit/5
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
            ViewData["FuncionarioId"] = new SelectList(_context.Set<Funcionario>(), "FuncionarioId", "FuncionarioNome", registro.FuncionarioId);
            ViewData["WBSList"] = _context.WBS.ToList();
            return View(registro);
        }

        // POST: Registroes/Edit/5
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
            ViewData["FuncionarioId"] = new SelectList(_context.Set<Funcionario>(), "FuncionarioId", "FuncionarioNome", registro.FuncionarioId);
            ViewData["WBSList"] = _context.WBS.ToList();
            return View(registro);
        }

        // GET: Registroes/Delete/5
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

        // POST: Registroes/Delete/5
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

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetWBSOptions()
        {
            var wbsOptions = await _context.WBS.Select(w => new { w.WBSId, w.Codigo }).ToListAsync();
            return Json(wbsOptions);
        }



        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] List<Registro> registros)
        {
            if (registros == null || !registros.Any())
            {
                return BadRequest("No data received");
            }

            foreach (var registro in registros)
            {
                registro.DataRegistro = DateTime.Parse(registro.DataRegistro.ToString());
                _context.Registro.Add(registro);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool RegistroExists(int id)
        {
            return _context.Registro.Any(e => e.RegistroId == id);
        }
    }
}
