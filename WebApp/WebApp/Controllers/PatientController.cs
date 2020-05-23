using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
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


		[BindProperty]
		public Patient Patient { get; set; }
		[BindProperty]
		public TimetableVisit TimetableVisit { get; set; }

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
				_db.Patient.Include(tp => tp.CurrenctDoctor).Where(patient => patient.NotPatientAnymore == false && patient.CurrenctDoctor.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();

			ViewBag.TreatedPatients = treatedPatients;

			ViewBag.AllDoctors = _db.User.Where(u => u.Pesel != User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
			ViewBag.SharedPatients = _db.SharedPatients.Include(sp => sp.Doctor).Include(sp => sp.Patient).ToList();

			ViewBag.Username = HttpContext.User.Identity.Name;
			return View(Patient);
		}

		[HttpPost("Index")]
		public async Task<IActionResult> CreatePatient()
		{
			Patient foundPatient = _db.Patient.Where(p => p.Pesel == Patient.Pesel).FirstOrDefault();

			if (foundPatient != null)
			{
				if (foundPatient.NotPatientAnymore)
				{
					foundPatient.NotPatientAnymore = false;
					await _db.SaveChangesAsync();
					_flasher.Flash(Types.Info, "Pomyślnie dodano nowego pacjenta.", dismissable: true); ;
				}
				else
					_flasher.Flash(Types.Danger, "Pacjent jest już zapisany w systemie.", dismissable: true);
			}
			else if (!PeselChecksum(Patient.Pesel))
			{
				_flasher.Flash(Types.Danger, "Podany pesel jest nieprawidłowy.", dismissable: true);
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

		public bool PeselChecksum(string pesel)
		{
			bool peselValid = false;

			if (pesel.Length == 11 && ulong.TryParse(pesel, out _))
			{
				int checksum = 0;

				int[] key = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
				for (int i = 0; i < key.Length; i++)
					checksum += (int)Char.GetNumericValue(pesel[i]) * key[i];

				if (10 - checksum % 10 == (int)Char.GetNumericValue(pesel[pesel.Length - 1]))
					peselValid = true;
			}

			return peselValid;
		}

		[HttpPost("Index/Delete")]
		public IActionResult SignOutPatient(int id, string action, string controller)
		{
			Patient foundPatient = _db.Patient.Where(p => p.Id == id).FirstOrDefault();

			foundPatient.NotPatientAnymore = true;

			_db.SaveChanges();

			SetPatientDeleteTimer(foundPatient.Id);

			_flasher.Flash(Types.Info, "Pomyślnie wypisano pacjenta.", dismissable: true);

			return RedirectToAction(action, controller);
		}

		private class PatientTimer : Timer
		{
			public int patientId;
		}

		private void SetPatientDeleteTimer(int patientId)
		{
			PatientTimer timer = new PatientTimer
			{
				Interval = 157680000000, // 5 years in miliseconds
				AutoReset = false,
				patientId = patientId
			};

			timer.Elapsed += DeletePatient;
			timer.Start();
		}

		private void DeletePatient(object source, ElapsedEventArgs e)
		{
			PatientTimer patientTimer = (PatientTimer)source;

			DbContextOptionsBuilder<ApplicationDbContext> op = new DbContextOptionsBuilder<ApplicationDbContext>();
			op.UseSqlServer("Server=DESKTOP-CVC40GK;Database=PlacowkaMedyczna;Trusted_Connection=True;MultipleActiveResultSets=True");

			var _db = new ApplicationDbContext(op.Options);

			Patient patientToDelete = _db.Patient.Where(p => p.Id == patientTimer.patientId).FirstOrDefault();

			_db.Patient.Remove(patientToDelete);
			_db.SaveChanges();

			patientTimer.Stop();
			patientTimer.Dispose();
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

		[HttpGet("Timetable")]
		public IActionResult Timetable()
		{
			List<Patient> treatedPatients =
				_db.Patient.Where(patient => patient.NotPatientAnymore == false && patient.CurrenctDoctor.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
			treatedPatients.AddRange(_db.SharedPatients.Where
				(sp => !sp.Patient.NotPatientAnymore &&
							sp.Doctor.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier))
				.Include
				(sp => sp.Patient).Select
				(sp => sp.Patient).ToList());
			ViewBag.TreatedPatients = treatedPatients;

			TimetableVisit = new TimetableVisit();
			return View(TimetableVisit);
		}

		[HttpPost("Timetable")]
		public async Task<IActionResult> CreateTimetableVisit(int patientId)
		{
			if (TimetableVisit.End < TimetableVisit.Start)
				_flasher.Flash(Types.Danger, "Źle wybrano daty.", dismissable: true);
			else
			{
				TimetableVisit.Patient = _db.Patient.Single(p => p.Id == patientId);
				_db.User.Single(u => u.Pesel == User.FindFirstValue(ClaimTypes.NameIdentifier)).Visits.Add(TimetableVisit);
				await _db.SaveChangesAsync();
			}

			return RedirectToAction("Timetable");
		}
	}
}
