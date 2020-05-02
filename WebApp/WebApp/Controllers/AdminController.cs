using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Admin")]
    public class AdminController : Controller
    {
	    private readonly ApplicationDbContext _db;

	    public AdminController(ApplicationDbContext db)
	    {
		    _db = db;
	    }

	    [HttpGet("Patients")]
        public IActionResult Patients()
        {
	        ViewBag.Patients = _db.Patient.Where(p => p.NotPatientAnymore == false).ToList();
	        ViewBag.Doctors = _db.User.Where(u => u.IsAdmin == false).ToList();
	        ViewBag.SharedPatients = _db.SharedPatients.ToList();
            return View();
        }
    }
}