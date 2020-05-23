using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
    public class TimetableController : ControllerBase
    {
	    private readonly ApplicationDbContext _db;

	    public TimetableController(ApplicationDbContext db)
	    {
		    _db = db;
	    }

	    [HttpGet]
        public IActionResult GetEvents()
        {
	        IEnumerable<TimetableVisit> visits = _db.User.
		        Include(u => u.Visits).Include("Visits.Patient")
		        .Where(u => u.Pesel == User.FindFirst(ClaimTypes.NameIdentifier).Value).Select(u => u.Visits).FirstOrDefault();
	        return Ok(visits);
        }
    }
}