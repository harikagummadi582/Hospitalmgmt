using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital_Management.Data;
using Hospital_Management.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Hospital_Management.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly HealthCareDbContext _context;

        public PatientsController(HealthCareDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            List<Patient> patients;

            if (role == "Patient")
            {
                patients = await _context.Patients
                    .Include(p => p.User)
                    .Where(p => p.UserID == Convert.ToInt32(userId)) 
                    .ToListAsync();
            }
            else if (role == "Admin")
            {
                patients = await _context.Patients
                    .Include(p => p.User) 
                    .ToListAsync();
            }
            else if (role == "Doctor")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserID == Convert.ToInt32(userId));
                if (doctor != null)
                {
                    patients = await _context.Appointments
                        .Include(a => a.Patient)
                        .Where(a => a.DoctorID == doctor.DoctorID)
                        .Select(a => a.Patient)
                        .Distinct()
                        .ToListAsync();
                }
                else
                {
                    patients = new List<Patient>();
                }
            }
            else
            {
                patients = new List<Patient>();
            }

            return View(patients);
        }


        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PatientID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            /*ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(userId);

            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", userId);
            ViewBag.UserName = user?.Username;
            ViewBag.UserEmail = user?.Email;*/
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientID,Name,DOB,Gender,BloodGroup,Address")] Patient patient)
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            patient.UserID = Convert.ToInt32(userId);

            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                TempData["success"] = "Patient tCreated Successfully";
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(x => x.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", patient.UserID);
            return View(patient);
        }
        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Patients == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewBag.UserName = patient.Name;
            
            return View(patient);
        }

        // POST: Patients/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientID,UserID,Name,DOB,Gender,BloodGroup,Address")] Patient patient)
        {
            if (id != patient.PatientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Patient details edited successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email", patient.UserID);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.UserID == id);
            patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            try
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Patient details deleted successfully" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = $"Failed to delete patient details: {ex.Message}" });
            }


        }

        private bool PatientExists(int id)
        {
          return (_context.Patients?.Any(e => e.PatientID == id)).GetValueOrDefault();
        }
    }
}
