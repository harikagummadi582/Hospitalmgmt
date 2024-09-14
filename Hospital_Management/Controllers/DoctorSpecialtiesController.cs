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
    public class DoctorSpecialtiesController : Controller
    {
        private readonly HealthCareDbContext _context;

        public DoctorSpecialtiesController(HealthCareDbContext context)
        {
            _context = context;
        }

        // GET: DoctorSpecialties
        public async Task<IActionResult> Index()
        {
            var healthCareDbContext = _context.DoctorSpecialties.Include(d => d.Doctor).Include(d => d.Specialty);
            return View(await healthCareDbContext.ToListAsync());
        }

        // GET: DoctorSpecialties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DoctorSpecialties == null)
            {
                return NotFound();
            }

            var doctorSpecialty = await _context.DoctorSpecialties
                .Include(d => d.Doctor)
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(m => m.DoctorSpecialtyID == id);
            if (doctorSpecialty == null)
            {
                return NotFound();
            }

            return View(doctorSpecialty);
        }

        // GET: DoctorSpecialties/Create
        public IActionResult Create()
        {
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name");
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorSpecialtyID,DoctorID,SpecialtyID")] DoctorSpecialty doctorSpecialty)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(doctorSpecialty);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Doctor Specialization added Successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    TempData["error"] = "Something went wrong";
                }
                
            }
            var errors = ModelState.Values.SelectMany(u => u.Errors);
            foreach(var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Gender", doctorSpecialty.DoctorID);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyName", doctorSpecialty.SpecialtyID);
            return View(doctorSpecialty);
        }

        // GET: DoctorSpecialties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DoctorSpecialties == null)
            {
                return NotFound();
            }

            var doctorSpecialty = await _context.DoctorSpecialties.FindAsync(id);
            if (doctorSpecialty == null)
            {
                return NotFound();
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name", doctorSpecialty.DoctorID);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyName", doctorSpecialty.SpecialtyID);
            return View(doctorSpecialty);
        }

        // POST: DoctorSpecialties/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorSpecialtyID,DoctorID,SpecialtyID")] DoctorSpecialty doctorSpecialty)
        {
            if (id != doctorSpecialty.DoctorSpecialtyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorSpecialty);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Doctor Specialization edited Successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["error"] = "Something went wrong while editing!!";
                    if (!DoctorSpecialtyExists(doctorSpecialty.DoctorSpecialtyID))
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
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name", doctorSpecialty.DoctorID);
            ViewData["SpecialtyID"] = new SelectList(_context.Specialties, "SpecialtyID", "SpecialtyName", doctorSpecialty.SpecialtyID);
            return View(doctorSpecialty);
        }

 

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speciality = await _context.DoctorSpecialties
                .FirstOrDefaultAsync(m => m.DoctorSpecialtyID == id);
            speciality = await _context.DoctorSpecialties.FindAsync(id);
            if (speciality == null)
            {
                return NotFound();
            }
            try
            {
                _context.DoctorSpecialties.Remove(speciality);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Doctor Specialization deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to delete Doctor Specialization: {ex.Message}" });
            }


        }

        private bool DoctorSpecialtyExists(int id)
        {
          return (_context.DoctorSpecialties?.Any(e => e.DoctorSpecialtyID == id)).GetValueOrDefault();
        }
    }
}
