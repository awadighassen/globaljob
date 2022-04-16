using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using globaljob.Data;
using globaljob.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace globaljob.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Admin
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index()
        {
            var UsersList = _context.Users.ToList();
            List<ApplicationUser> ValidUsersList = new();

            foreach(ApplicationUser user in UsersList)
            {
                if (! await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                {
                    ValidUsersList.Add(user);
                }
            }
            return View(ValidUsersList);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ListUsersToPromote()
        {
            var UsersList = _context.Users.ToList();
            List<ApplicationUser> ValidUsersList = new();

            foreach (ApplicationUser user in UsersList)
            {
                if (!await _userManager.IsInRoleAsync(user, "SuperAdmin") && !await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    ValidUsersList.Add(user);
                }
            }
            return View(ValidUsersList);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignAdmin(string id)
        {

            var user = _userManager.FindByIdAsync(id).Result;
            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Details/5
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

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection form) 
        {
            ApplicationUser user = null;
            if (ModelState.IsValid)
            {
                user = new ApplicationUser { UserName = form["Email"].ToString(), Email = form["Email"].ToString(), FirstName = form["FirstName"].ToString(), LastName = form["LastName"].ToString(),Address = form["Address"].ToString()};
                user.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(user, form["Password"].ToString());
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
               
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Edit/5
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

        // POST: Admin/Edit/5
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

        // GET: Admin/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidatureExists(int id)
        {
            return _context.Candidature.Any(e => e.Id == id);
        }
    }
}
