using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using globaljob.Data;
using globaljob.Models;
using Microsoft.AspNetCore.Identity;

namespace globaljob.Controllers
{
    public class OffreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OffreController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Offre
        public async Task<IActionResult> Index()
        {
            var offres = await _context.Offre.ToListAsync();
            List<Offre> offresRecruteur = new();
            var UserId = _userManager.GetUserId(HttpContext.User);

            foreach (Offre offre in offres)
            {
                if (offre.ApplicationUserId is not null && offre.ApplicationUserId.Equals(UserId))
                {
                    offresRecruteur.Add(offre);
                }

            }

            return View(offresRecruteur);
        }

        public async Task<IActionResult> Consulter_Offres()
        {
            return View(await _context.Offre.ToListAsync());
        }

        public async Task<IActionResult> Details_Offre(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offre
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }
        // GET: Offre/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offre
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }

        // GET: Offre/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Offre/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatePublication,DateFin,Description,Titre")] Offre offre)
        {
            offre.ApplicationUserId = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {
                _context.Add(offre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(offre);
        }

        // GET: Offre/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offre.FindAsync(id);
            if (offre == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", offre.ApplicationUserId);
            return View(offre);
        }

        // POST: Offre/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatePublication,DateFin,Description,Titre")] Offre offre)
        {
            offre.ApplicationUserId = _userManager.GetUserId(HttpContext.User);
            if (id != offre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OffreExists(offre.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", offre.ApplicationUserId);
            return View(offre);
        }

        // GET: Offre/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offre
                .Include(o => o.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }

        // POST: Offre/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offre = await _context.Offre.FindAsync(id);
            var candidature = await _context.Candidature
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(m => m.OffreId == id);

            _context.Offre.Remove(offre);

            if (candidature is not null)
            {
                _context.Candidature.Remove(candidature);
            }
                
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OffreExists(int id)
        {
            return _context.Offre.Any(e => e.Id == id);
        }
    }
}
