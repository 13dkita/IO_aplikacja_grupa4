using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Specialization { get; set; }
		[Required]
		[RegularExpression(@"^(\d){11}$", ErrorMessage = "Pesel musi się składać z jedenastu cyfr.")]
		public string Pesel { get; set; }
		[Required]
		public string Password { get; set; }
		public bool IsAdmin { get; set; }
	}
}
