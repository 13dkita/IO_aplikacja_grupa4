using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Controllers
{
	[Route("Patient")]
	[Authorize(Roles = "Admin, User")]
	public class PatientController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IFlasher _flasher;

		private string currentDoctorsPesel;

		[BindProperty]
		public Patient Patient { get; set; }

		public PatientController(ApplicationDbContext db, IFlasher flasher)
		{
			_db = db;
			_flasher = flasher;
		}

		[HttpGet("Index")]
		public IActionResult Index()
		{
			if (User.FindFirstValue(ClaimTypes.Role) == "Admin")
				return RedirectToAction("RegisterDoctor", "Register");

			Patient = new Patient();

			List<Patient> treatedPatients =
				_db.Patient.Where(patient => patient.NotPatientAnymore == false && patient.CurrenctDoctor.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();

			ViewBag.TreatedPatients = treatedPatients;

			ViewBag.AllDoctors = _db.User.Where(u => u.Pesel != User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();

			ViewBag.Username = HttpContext.User.Identity.Name;
			return View(Patient);
		}

		[HttpPost("Index")]
		public async Task<IActionResult> CreatePatient()
		{
			Patient foundPatient = _db.Patient.Where(p => p.Pesel == Patient.Pesel).FirstOrDefault();

			if (foundPatient != null)
			{
				_flasher.Flash(Types.Info, "Pacjent jest już zapisany w systemie.", dismissable: true);
			}
			else
			{
				Patient.DateCreated = DateTime.Now;
				Patient.CurrenctDoctor = _db.User.Where(usr => usr.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier))
					.Select(x => x).First();
				Patient.RoentgenPhoto = RoentgenGenerator.LoadRandomImage();

				TreatmentHistory th = new TreatmentHistory
				{
					Doctor = Patient.CurrenctDoctor,
					Patient = Patient,
					TreatmentDate = DateTime.Now
				};

				await _db.Patient.AddAsync(Patient);
				await _db.TreatmentHistory.AddAsync(th);
				await _db.SaveChangesAsync();

				_flasher.Flash(Types.Info, "Pomyślnie dodano nowego pacjenta.", dismissable: true);
			}

			return RedirectToAction("Index");
		}

		[HttpPost("Index/Delete")]
		public async Task<IActionResult> SignOutPatient(int id, string action, string controller)
		{
			Patient foundPatient = _db.Patient.Where(p => p.Id == id).FirstOrDefault();

			foundPatient.NotPatientAnymore = true;

			await _db.SaveChangesAsync();

			_flasher.Flash(Types.Info, "Pomyślnie wypisano pacjenta.", dismissable: true);

			return RedirectToAction(action, controller);
		}

		[HttpPost("Index/Share")]
		public async Task<IActionResult> SharePatient(int patientId, int? doctorId, string action, string controller)
		{
			SharedPatients foundSharedPatient =
				_db.SharedPatients.FirstOrDefault(sp => sp.Doctor.Id == doctorId && sp.Patient.Id == patientId);

			if (doctorId == null)
				_flasher.Flash(Types.Danger, "Musisz wybrać lekarza, któremu chcesz udostępnić pacjenta.", dismissable: true);

			else if (foundSharedPatient != null)
				_flasher.Flash(Types.Danger, "Pacjent został już udostępniony wybranemu lekarzowi.", dismissable: true);
			
			else
			{
				SharedPatients sharedPatient = new SharedPatients
				{
					Doctor = _db.User.FirstOrDefault(d => d.Id == doctorId),
					Patient = _db.Patient.FirstOrDefault(p => p.Id == patientId)
				};

				_db.SharedPatients.Add(sharedPatient);

				await _db.SaveChangesAsync();

				_flasher.Flash(Types.Info, "Pomyślnie udostępniono pacjenta.", dismissable: true);
			}

			
			return RedirectToAction(action, controller);
		}

		[HttpGet("Shared")]
		public IActionResult SharedPatients()
		{
			List<Patient> treatedPatients =
				_db.SharedPatients.Where(sp =>
					sp.Patient.NotPatientAnymore == false &&
					sp.Doctor.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier)).Select(sp => sp.Patient).ToList();

			ViewBag.TreatedPatients = treatedPatients;

			ViewBag.Username = HttpContext.User.Identity.Name;
			return View();
		}
	}
}
