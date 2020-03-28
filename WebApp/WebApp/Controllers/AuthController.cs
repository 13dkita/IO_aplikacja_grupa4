using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
	public class AuthController : Controller
	{

		private readonly ApplicationDbContext _db;

		public AuthController(ApplicationDbContext db)
		{
			_db = db;
		}

		[BindProperty] public TempUser TempUser { get; set; }
		[BindProperty] public User User { get; set; }

		[HttpGet("RegisterDoctor")]
		public IActionResult RegisterDoctor()
		{
			TempUser = new TempUser();
			return View(TempUser);
		}

		[HttpPost("RegisterDoctor")]
		public async Task<IActionResult> CreateDoctor()
		{
			if (!ModelState.IsValid)
				return RedirectToAction("RegisterDoctor");

			await _db.TempUser.AddAsync(TempUser);
			await _db.SaveChangesAsync();

			return RedirectToAction("Index", "Home");
		}

		[HttpGet("RegisterSelf")]
		public async Task<IActionResult> RegisterSelf()
		{
			User = new User();
			return View(User);
		}

		[HttpPost("RegisterSelf")]
		public async Task<IActionResult> PostRegisterSelf(string repeatPassword)
		{
			TempUser foundUser = (from u in _db.TempUser where u.Pesel == User.Pesel select u).FirstOrDefault();

			if (foundUser == null)
				return RedirectToAction("RegisterSelf", "Auth");
			else
			{
				if (User.Password == repeatPassword)
				{
					User.FirstName = foundUser.FirstName;
					User.LastName = foundUser.LastName;
					User.Specialization = foundUser.Specialization;

					await _db.User.AddAsync(User);
					_db.TempUser.Remove(foundUser);
					await _db.SaveChangesAsync();
				}

				else
					return RedirectToAction("RegisterSelf", "Auth");
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet("Login")]
		public IActionResult Login()
		{
			User = new User();
			return View();
		}

		[HttpPost("Login")]
		public IActionResult PostLogin()
		{
			User foundUser =
				(from u in _db.User
				 where u.Pesel == User.Pesel && u.Password == User.Password
				 select u).FirstOrDefault();

			if (foundUser != null)
			{
				ClaimsIdentity identity = null;

				if (foundUser.IsAdmin)
				{
					identity = new ClaimsIdentity(new[] {
						new Claim(ClaimTypes.Name, $"{foundUser.FirstName} {foundUser.LastName}"),
						new Claim(ClaimTypes.Role, "Admin")
					}, CookieAuthenticationDefaults.AuthenticationScheme);
				}
				else
				{
					identity = new ClaimsIdentity(new[] {
						new Claim(ClaimTypes.Name, $"{foundUser.FirstName} {foundUser.LastName}"),
						new Claim(ClaimTypes.Role, "User")
					}, CookieAuthenticationDefaults.AuthenticationScheme);
				}

				var principal = new ClaimsPrincipal(identity);
				var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				if (foundUser.IsAdmin)
					return RedirectToAction("", "");

				return RedirectToAction("Index", "Home");
			}

			return RedirectToAction("Login");
		}

		[HttpPost("Logout")]
		public IActionResult Logout()
		{
			var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}


	}
}