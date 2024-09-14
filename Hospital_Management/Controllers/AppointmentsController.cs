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
using YourProject.Attributes;

namespace Hospital_Management.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly HealthCareDbContext _context;
        private int appointmentID = 0;
        public AppointmentsController(HealthCareDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);



            IQueryable<Appointment> appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient);

            if (role == "Doctor")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserID == userId);
                if (doctor != null)
                {
                    appointments = appointments.Where(a => a.DoctorID == doctor.DoctorID);
                }
            }
            else if (role == "Patient")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserID == userId);
                if (patient != null)
                {
                    appointments = appointments.Where(a => a.PatientID == patient.PatientID);
                }
            }

            return View(await appointments.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name");
            
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "Name");
            ViewBag.UserName = User.FindFirstValue(ClaimTypes.Name);
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentID,PatientID,DoctorID,AppointmentDate,AppointmentTime,Status,CancellationReason,PatientHealthIssues")] Appointment appointment)
        {
            appointment.Status = "Registered";

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(u => u.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name", appointment.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "Name", appointment.PatientID);

            return View(appointment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            appointmentID = (int)id;
            if (id == null || _context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name", appointment.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "Name", appointment.PatientID);
            return View(appointment);
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentID,PatientID,DoctorID,AppointmentDate,AppointmentTime,Status,CancellationReason,PatientHealthIssues")] Appointment appointment)
        {
            var patientId = await _context.Appointments
                .Where(a => a.AppointmentID == appointment.AppointmentID)
                .Select(a => a.PatientID)
                .FirstOrDefaultAsync();
            appointment.PatientID = patientId;
            if (id != appointment.AppointmentID)
            {
                return NotFound();
            }
           
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentID))
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
            var errors = ModelState.Values.SelectMany(u => u.Errors);
            foreach(var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            ViewData["DoctorID"] = new SelectList(_context.Doctors, "DoctorID", "Name", appointment.DoctorID);
            ViewData["PatientID"] = new SelectList(_context.Patients, "PatientID", "Name", appointment.PatientID);
            return View(appointment);
        }

      

        // GET: Appointments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointments = await _context.Appointments
                .FirstOrDefaultAsync(m => m.AppointmentID == id);
            appointments = await _context.Appointments.FindAsync(id);
            if (appointments == null)
            {
                return NotFound();
            }
            try
            {
                _context.Appointments.Remove(appointments);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Appointmnet deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Failed to delete Appointment: {ex.Message}" });
            }


        }


        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return Json(new { success = false, message = "Appointment not found" });
            }

            appointment.Status = "Cancelled";
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Appointment cancelled successfully" });
        }


        private bool AppointmentExists(int id)
        {
            return (_context.Appointments?.Any(e => e.AppointmentID == id)).GetValueOrDefault();
        }
    }
}
