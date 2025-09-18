using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize] 
    public class InquilinoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InquilinoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Inquilinos.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dni,Nombre,Apellido,Email,Telefono")] Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inquilino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var inquilino = await _context.Inquilinos.FindAsync(id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Dni,Nombre,Apellido,Email,Telefono")] Inquilino inquilino)
        {
            if (id != inquilino.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inquilino);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inquilinos.Any(e => e.Id == inquilino.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var inquilino = await _context.Inquilinos.FirstOrDefaultAsync(m => m.Id == id);
            if (inquilino == null) return NotFound();
            return View(inquilino);
        }

        // POST: Inquilino/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inquilino = await _context.Inquilinos.FindAsync(id);
            if (inquilino != null)
            {
                _context.Inquilinos.Remove(inquilino);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}