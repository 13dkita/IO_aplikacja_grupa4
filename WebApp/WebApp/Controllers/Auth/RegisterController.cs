using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Auth/Register")]
	public class RegisterController : Controller
	{

		private readonly ApplicationDbContext _db;
		private readonly IFlasher _flasher;

		public RegisterController(ApplicationDbContext db, IFlasher flasher)
		{
			_db = db;
			_flasher = flasher;
		}

		[BindProperty] public TempUser TempUser { get; set; }

		[BindProperty] public User User { get; set; }

		[Authorize(Roles = "Admin")]
		[HttpGet("RegisterDoctor")]
		public async Task<IActionResult> RegisterDoctor()
		{
			ViewBag.AllDoctors = await _db.User.ToListAsync();
			TempUser = new TempUser();
			return View(TempUser);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost("RegisterDoctor")]
		public async Task<IActionResult> CreateDoctor()
		{
			TempUser foundTempUser = (from tmpUsr in _db.TempUser where tmpUsr.Pesel == TempUser.Pesel select tmpUsr).FirstOrDefault();
			User foundUser = (from usr in _db.User where usr.Pesel == TempUser.Pesel select usr).FirstOrDefault();

			
			if (foundTempUser != null || foundUser != null)
			{
				_flasher.Flash(Types.Danger, "Lekarz o podanym numer PESEL już istnieje.", dismissable: true);
			}
			else if (!PeselChecksum(TempUser.Pesel))
			{
				_flasher.Flash(Types.Danger, "Podany pesel jest nieprawidłowy.", dismissable: true);
			}
			else
			{
				await _db.TempUser.AddAsync(TempUser);
				await _db.SaveChangesAsync();
				_flasher.Flash(Types.Success, "Pomyślnie zarejestrowano lekarza.", dismissable: true);
			}

			return RedirectToAction("RegisterDoctor");
		}

		public bool PeselChecksum(string pesel)
		{
			bool peselValid = false;

			if (pesel.Length == 11 && ulong.TryParse(pesel, out _))
			{
				int checksum = 0;

				int[] key = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
				for (int i = 0; i < key.Length; i++)
					checksum += (int) Char.GetNumericValue(pesel[i]) * key[i];

				if (10 - checksum % 10 == (int) Char.GetNumericValue(pesel[pesel.Length - 1]))
					peselValid = true;
			}

			return peselValid;
		}

		[HttpGet("RegisterSelf")]
		public  IActionResult RegisterSelf()
		{
			User = new User();
			return View(User);
		}

		[HttpPost("RegisterSelf")]
		public async Task<IActionResult> PostRegisterSelf(string repeatPassword)
		{
			TempUser foundTempUser = (from u in _db.TempUser where u.Pesel == User.Pesel select u).FirstOrDefault();
			User user = (from u in _db.User where u.Pesel == User.Pesel select u).FirstOrDefault();

			if (foundTempUser == null && user == null)
			{
				_flasher.Flash(Types.Info, "Nie odnaleziono użytkownika o podanym numerze PESEL. Skontakuj się z administratorem.", dismissable: true);
				return RedirectToAction("RegisterSelf");
			}
			else
			{
				if (User.Password != repeatPassword)
				{
					_flasher.Flash(Types.Danger, "Powtórzone hasło nie zgadza się.", dismissable: true);
					return RedirectToAction("RegisterSelf");
				}
				else if (user != null)
				{
					_flasher.Flash(Types.Info, "Pomyślnie zmieniono hasło.", dismissable: true);
					user.Password = User.Password;
					await _db.SaveChangesAsync();
				}
				else
				{
					_flasher.Flash(Types.Success, "Rejestracja zakończona sukcesem.", dismissable: true);
					User.FirstName = foundTempUser.FirstName;
					User.LastName = foundTempUser.LastName;
					User.Specialization = foundTempUser.Specialization;
					User.DateCreated = DateTime.Now;

					await _db.User.AddAsync(User);
					_db.TempUser.Remove(foundTempUser);
					await _db.SaveChangesAsync();
				}
			}

			return RedirectToAction("Login", "Login");
		}

		[HttpGet("DeleteDoctor/{id:int}")]
		public async Task<IActionResult> DeleteDoctor(int id)
		{
			User foundDoctor = _db.User.Where(p => p.Id == id).FirstOrDefault();
			List<SharedPatients> sp = _db.SharedPatients.Where(sp => sp.Doctor.Id == id).ToList();
			List<Patient> patients = _db.Patient.Where(p => p.CurrenctDoctor.Id == id).ToList();

			foreach (var patient in patients)
			{
				patient.CurrenctDoctor = null;
			}

			_db.SharedPatients.RemoveRange(sp);
			_db.User.Remove(foundDoctor);
			await _db.SaveChangesAsync();

			_flasher.Flash(Types.Info, "Pomyślnie wypisano lekarza.", dismissable: true);

			return RedirectToAction("RegisterDoctor");
		}

		[HttpPost("ChangeDoctorPassword/{id:int}")]
		public async Task<IActionResult> ChangeDoctorPassword(int id, string repeatPassword, string password)
		{
			if (password != repeatPassword)
				_flasher.Flash(Types.Danger, "Powtórzone hasło nie zgadza się.", dismissable: true);
			else
			{
				User doctor = _db.User.Where(u => u.Id == id).FirstOrDefault();
				doctor.Password = password;
				await _db.SaveChangesAsync();

				_flasher.Flash(Types.Success, "Pomyślnie zmieniono hasło lekarza.", dismissable: true);
			}



			return RedirectToAction("RegisterDoctor");
		}
	}
}