using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
	public class User
	{
		public User()
		{
			Visits = new List<TimetableVisit>();
		}

		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Imię jest wymagane.")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Nazwisko jest wymagane.")]
		public string LastName { get; set; }
		[Required(ErrorMessage = "Specjalizacja jest wymagana.")]
		public string Specialization { get; set; }
		[Required(ErrorMessage = "Pesel jest wymagany.")]
		[RegularExpression(@"^(\d){11}$", ErrorMessage = "PESEL musi się składać z jedenastu cyfr.")]
		public string Pesel { get; set; }
		[Required]
		[MinLength(8, ErrorMessage = "Hasło musi zawierać przynajmniej 8 znaków.")]
		public string Password { get; set; }
		public bool IsAdmin { get; set; }
		[Required]
		public DateTime DateCreated
		{
			get
			{
				return this.dateCreated.HasValue
					? this.dateCreated.Value
					: DateTime.Now;
			}

			set { this.dateCreated = value; }
		}

		private DateTime? dateCreated = null;

		public ICollection<TimetableVisit> Visits { get; set; }


	}
}

