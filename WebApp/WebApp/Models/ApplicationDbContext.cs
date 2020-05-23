using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace WebApp.Models
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public Microsoft.EntityFrameworkCore.DbSet<User> User { get; set; }
		public Microsoft.EntityFrameworkCore.DbSet<TempUser> TempUser { get; set; }
		public Microsoft.EntityFrameworkCore.DbSet<Patient> Patient { get; set; }
		public Microsoft.EntityFrameworkCore.DbSet<TreatmentHistory> TreatmentHistory { get; set; }
		public Microsoft.EntityFrameworkCore.DbSet<SharedPatients> SharedPatients{ get; set; }
		public Microsoft.EntityFrameworkCore.DbSet<TimetableVisit> TimetableVisits { get; set; }
		
	}
}
