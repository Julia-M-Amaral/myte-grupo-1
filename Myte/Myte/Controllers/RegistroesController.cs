using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Myte.Data;
using Myte.Models;

namespace Myte.Controllers
{
    [Authorize(Policy = "RequerFuncOuAdmin")]
    public class RegistroesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RegistroesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var funcionario = await _context.Funcionario.FirstOrDefaultAsync(f => f.Email == user.Email);

            if (funcionario == null)
            {
                return NotFound("Funcionário não encontrado.");
            }

            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Codigo");
            return View(new Registro { FuncionarioId = funcionario.FuncionarioId });
        }

        // POST: Registroes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistroId,WBSId,HorasTrab,DataRegistro")] Registro registro)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var funcionario = await _context.Funcionario.FirstOrDefaultAsync(f => f.Email == user.Email);

                if (funcionario == null)
                {
                    return NotFound("Funcionário não encontrado.");
                }

                registro.FuncionarioId = funcionario.FuncionarioId;
                _context.Add(registro);
                await _context.SaveChangesAsync();
                TempData["message"] = "REGISTRO CADASTRADO COM SUCESSO";
                return RedirectToAction(nameof(Index));
            }

            ViewData["WBSId"] = new SelectList(_context.WBS, "WBSId", "Codigo", registro.WBSId);
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
            var wbsList = _context.WBS.Select(w => new { w.WBSId, w.Codigo }).ToList();
            ViewData["WBSId"] = new SelectList(wbsList, "WBSId", "Codigo", registro.WBSId);
            return View(registro);
        }

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
                    if (!RegistroExiste(registro.RegistroId))
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
            var wbsList = _context.WBS.Select(w => new { w.WBSId, w.Codigo }).ToList();
            ViewData["WBSId"] = new SelectList(wbsList, "WBSId", "Codigo", registro.WBSId);
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
        public async Task<IActionResult> ObterOpcoesWBS()
        {
            var opcoesWBS = await _context.WBS.Select(w => new { w.WBSId, w.Codigo }).ToListAsync();
            return Json(opcoesWBS);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarDados([FromBody] List<Registro> registros)
        {
            if (registros == null || !registros.Any())
            {
                Console.WriteLine("Nenhum dado recebido.");
                return BadRequest("No data received");
            }

            foreach (var registro in registros)
            {
                Console.WriteLine($"Recebido: FuncionarioId = {registro.FuncionarioId}, WBSId = {registro.WBSId}, HorasTrab = {registro.HorasTrab}, DataRegistro = {registro.DataRegistro}");
                _context.Registro.Add(registro);
            }
            await _context.SaveChangesAsync();
            TempData["message"] = "REGISTRO CADASTRADO COM SUCESSO";
            return Ok();
        }



        private bool RegistroExiste(int id)
        {
            return _context.Registro.Any(e => e.RegistroId == id);
        }
    }
}
