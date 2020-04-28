using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<User> User { get; set; }
		public DbSet<TempUser> TempUser { get; set; }
		public DbSet<Patient> Patient { get; set; }
		public DbSet<TreatmentHistory> TreatmentHistory { get; set; }
		public DbSet<SharedPatients> SharedPatients{ get; set; }
		
	}
}
