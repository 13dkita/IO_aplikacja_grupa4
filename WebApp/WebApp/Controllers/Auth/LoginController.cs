using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Flash;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Auth")]
	public class LoginController : Controller
	{

		private readonly ApplicationDbContext _db;
		private readonly IFlasher _flasher;

		public LoginController(ApplicationDbContext db, IFlasher flasher)
		{
			_db = db;
			_flasher = flasher;
		}

		[BindProperty] public User User { get; set; }


		[HttpGet("Login")]
		public IActionResult Login()
		{
			User = new User();
			return View(User);
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
						new Claim(ClaimTypes.Role, "Admin"),
						new Claim(ClaimTypes.NameIdentifier, foundUser.Pesel)
					}, CookieAuthenticationDefaults.AuthenticationScheme);
				}
				else
				{
					identity = new ClaimsIdentity(new[] {
						new Claim(ClaimTypes.Name, $"{foundUser.FirstName} {foundUser.LastName}"),
						new Claim(ClaimTypes.Role, "User"),
						new Claim(ClaimTypes.NameIdentifier, foundUser.Pesel)
					}, CookieAuthenticationDefaults.AuthenticationScheme);
				}

				var principal = new ClaimsPrincipal(identity);
				var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				if (foundUser.IsAdmin)
				{
					_flasher.Flash(Types.Info, "Zalogowano pomyślnie.", dismissable: true);
					return RedirectToAction("RegisterDoctor", "Register");
				}

				_flasher.Flash(Types.Info, "Zalogowano pomyślnie.", dismissable: true);
				return RedirectToAction("Index", "Home");
			}

			_flasher.Flash(Types.Danger, "PESEL lub hasło niepoprawne.", dismissable: true);
			return RedirectToAction("Login");
		}

		[Authorize(Roles = "Admin, User")]
		[HttpGet("Logout")]
		public IActionResult Logout()
		{
			var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			_flasher.Flash(Types.Info, "Wylogowano pomyślnie.", dismissable: true);
			return RedirectToAction("Login");
		}
	}
}