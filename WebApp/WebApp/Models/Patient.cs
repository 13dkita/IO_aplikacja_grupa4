using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Utils;

namespace WebApp.Models
{
	public class Patient
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Imię jest wymagane.")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Nazwisko jest wymagane.")]
		public string LastName { get; set; }
		[Required(ErrorMessage = "Wybranie płci jest wymagane")]
		public char SelectedGender { get; set; }
		[Required(ErrorMessage = "Pesel jest wymagany.")]
		[RegularExpression(@"^(\d){11}$", ErrorMessage = "PESEL musi się składać z jedenastu cyfr.")]
		public string Pesel { get; set; }
		public List<User> TreatmentHistory { get; set; }

		public User CurrenctDoctor { get; set; }

		public byte[] RoentgenPhoto
		{
			get { return RoentgenGenerator.LoadRandomImage(); }
			set { this.roentgenPhoto = value; }
		}

		private byte[] roentgenPhoto = null;

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
	}
}

