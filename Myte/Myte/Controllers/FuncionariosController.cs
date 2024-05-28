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
using Myte.Models.Enums;

namespace Myte.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; 

        public FuncionariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Funcionario.Include(f => f.Departamento);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Funcionarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionario
                .Include(f => f.Departamento)
                .FirstOrDefaultAsync(m => m.FuncionarioId == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // GET: Funcionarios/Create
        public IActionResult Create()
        {
            ViewData["DepartamentoId"] = new SelectList(_context.Set<Departamento>(), "DepartamentoId", "DepartamentoNome");
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(FuncionarioStatus)).Cast<FuncionarioStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text");
            return View();
        }

        // POST: Funcionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FuncionarioId,FuncionarioNome,Email,Senha,DataContratacao,DepartamentoId,NivelAcesso,Status")] Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = funcionario.FuncionarioNome,
                    Email = funcionario.Email,
                    EmailConfirmed = true // Confirmação de email definida como true
                };

                var result = await _userManager.CreateAsync(user, funcionario.Senha);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, funcionario.NivelAcesso ?? "Funcionario");
                    _context.Add(funcionario);
                    await _context.SaveChangesAsync();

                    TempData["message"] = "FUNCIONÁRIO CRIADO COM SUCESSO";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Set<Departamento>(), "DepartamentoId", "DepartamentoNome", funcionario.DepartamentoId);
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(FuncionarioStatus)).Cast<FuncionarioStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", funcionario.Status);
            return View(funcionario);
        }



        // GET: Funcionarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionario.FindAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }
            ViewData["DepartamentoId"] = new SelectList(_context.Set<Departamento>(), "DepartamentoId", "DepartamentoNome", funcionario.DepartamentoId);
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(FuncionarioStatus)).Cast<FuncionarioStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", funcionario.Status);
            return View(funcionario);
        }

        // POST: Funcionarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FuncionarioId,FuncionarioNome,Email,Senha,DataContratacao,DepartamentoId,NivelAcesso,Status")] Funcionario funcionario)
        {
            if (id != funcionario.FuncionarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcionario);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "FUNCIONÁRIO EDITADO COM SUCESSO";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionarioExists(funcionario.FuncionarioId))
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
            ViewData["DepartamentoId"] = new SelectList(_context.Set<Departamento>(), "DepartamentoId", "DepartamentoNome", funcionario.DepartamentoId);
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(FuncionarioStatus)).Cast<FuncionarioStatus>().Select(e => new { Value = e, Text = e.ToString() }), "Value", "Text", funcionario.Status);
            return View(funcionario);
        }

        // GET: Funcionarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcionario = await _context.Funcionario
                .Include(f => f.Departamento)
                .FirstOrDefaultAsync(m => m.FuncionarioId == id);
            if (funcionario == null)
            {
                return NotFound();
            }

            return View(funcionario);
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _context.Funcionario.FindAsync(id);
            if (funcionario != null)
            {
                _context.Funcionario.Remove(funcionario);
            }

            await _context.SaveChangesAsync();
            TempData["message"] = "FUNCIONÁRIO DELETADO COM SUCESSO";

            return RedirectToAction(nameof(Index));
        }

        private bool FuncionarioExists(int id)
        {
            return _context.Funcionario.Any(e => e.FuncionarioId == id);
        }
    }
}
