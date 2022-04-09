using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using globaljob.Data;
using globaljob.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;

namespace globaljob.Controllers
{
    public class CandidatureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public static int offreId;

        public CandidatureController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Candidature
        public async Task<IActionResult> Index()
        {
            var candidatures = await _context.Candidature.ToListAsync();
            List<Candidature> candidaturesOffre = new();

            foreach (Candidature candidature in candidatures)
            {
                if (candidature.OffreId.Equals(offreId))
                {
                    candidaturesOffre.Add(candidature);
                }
            }

            return View(candidaturesOffre);
        }

        // GET: Candidature/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidature = await _context.Candidature
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidature == null)
            {
                return NotFound();
            }

            return View(candidature);
        }

        // GET: Candidature/Create
        public  IActionResult Create()
        {
            var user = _userManager.FindByIdAsync(_userManager.GetUserId(HttpContext.User)).Result;
            ViewData["FullName"] = user.FirstName + " " + user.LastName;
            ViewData["UserEmail"] = user.Email;
            ViewData["UserPhoneNumber"] = user.PhoneNumber;
            //if (id is not null)
            //{
            //    offreId = (int)id;
            //}

            // ViewData["offreId"] = offreId;

            return View();
        }

        // POST: Candidature/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidature candidature) //[Bind("Id,Name,Email,PhoneNumber,ResumeURL,OffreId")] 
        {
           candidature.OffreId = _context.Offre.Find(offreId).Id;
            if (ModelState.IsValid)
            {
                if (candidature.Resume != null)
                {
                    string folder = "resumes/";
                    candidature.ResumeURL = await UploadImage(folder, candidature.Resume);
                }
                _context.Add(candidature);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Postulation enregistrée avec succès";
                return RedirectToAction(nameof(Create));
            }
            return View();
        }

        // GET: Candidature/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidature = await _context.Candidature.FindAsync(id);
            if (candidature == null)
            {
                return NotFound();
            }
            ViewData["OffreId"] = new SelectList(_context.Offre, "Id", "Description", candidature.OffreId);
            return View(candidature);
        }

        // POST: Candidature/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,ResumeURL,OffreId")] Candidature candidature)
        {
            if (id != candidature.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(candidature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandidatureExists(candidature.Id))
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
            ViewData["OffreId"] = new SelectList(_context.Offre, "Id", "Description", candidature.OffreId);
            return View(candidature);
        }

        // GET: Candidature/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidature = await _context.Candidature
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidature == null)
            {
                return NotFound();
            }

            return View(candidature);
        }

        // POST: Candidature/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidature = await _context.Candidature.FindAsync(id);
            _context.Candidature.Remove(candidature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidatureExists(int id)
        {
            return _context.Candidature.Any(e => e.Id == id);
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }

    }
}
