using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital_Management.Data;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authorization;
using YourProject.Attributes;

namespace Hospital_Management.Controllers
{
    [Authorize]
    [AdminOnly]
    public class SpecialtiesController : Controller
    {
        private readonly HealthCareDbContext _context;

        public SpecialtiesController(HealthCareDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
              return _context.Specialties != null ? 
                          View(await _context.Specialties.ToListAsync()) :
                          Problem("Entity set 'HealthCareDbContext.Specialties'  is null.");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Specialties == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(m => m.SpecialtyID == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpecialtyID,SpecialtyName")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {

                _context.Add(specialty);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Specialization Created Successfully";
                
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View(specialty);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Specialties == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            return View(specialty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SpecialtyID,SpecialtyName")] Specialty specialty)
        {
            if (id != specialty.SpecialtyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Specialization Edited Successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.SpecialtyID))
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
            return View(specialty);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speciality = await _context.Specialties
                .FirstOrDefaultAsync(m => m.SpecialtyID == id);
            speciality = await _context.Specialties.FindAsync(id);
            if (speciality == null)
            {
                return NotFound();
            }
            try
            {
                _context.Specialties.Remove(speciality);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Specialization deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to delete Specialization: {ex.Message}" });
            }
        }

        private bool SpecialtyExists(int id)
        {
          return (_context.Specialties?.Any(e => e.SpecialtyID == id)).GetValueOrDefault();
        }
    }
}
