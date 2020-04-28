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
			Patient = new Patient();

			List<Patient> treatedPatients =
				_db.Patient.Where(patient => patient.CurrenctDoctor.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();

			ViewBag.TreatedPatients = treatedPatients;

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
				Patient.TreatmentHistory = new List<User>();
				Patient.CurrenctDoctor = _db.User.Where(usr => usr.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier))
					.Select(x => x).First();
				Patient.RoentgenPhoto = RoentgenGenerator.LoadRandomImage();

				await _db.Patient.AddAsync(Patient);
				await _db.SaveChangesAsync();

				_flasher.Flash(Types.Info, "Pomyślnie dodano nowego pacjenta.", dismissable: true);
			}

			return RedirectToAction("Index");
		}
	}
}
