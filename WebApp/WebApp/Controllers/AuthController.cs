using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		    return View(TempUser);
	    }

		[HttpPost("RegisterSelf")]
	    public async Task<IActionResult> PostRegisterSelf(string password, string repeatPassword)
	    {
		    TempUser foundUser = (from u in _db.TempUser where u.Pesel == TempUser.Pesel select u).FirstOrDefault();

		    if (foundUser == null)
			    return RedirectToAction("RegisterSelf", "Auth");
		    else
		    {
			    if (password == repeatPassword)
			    {
				    User user = new User()
				    {
					    FirstName = foundUser.FirstName,
					    LastName = foundUser.LastName,
					    Pesel = foundUser.Pesel,
					    Password = password,
					    Specialization = foundUser.Specialization
				    };

				    await _db.User.AddAsync(user);
				    _db.TempUser.Remove(foundUser);
				    await _db.SaveChangesAsync();
			    }

			    else
				    return RedirectToAction("Login", "Auth");
		    }

		    return RedirectToAction("Index", "Home");
	    }

	    
	}
}