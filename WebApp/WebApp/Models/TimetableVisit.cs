using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
	public class TimetableVisit
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Data rozpoczęcia wizyty jest wymagana.")]
		public DateTime Start { get; set; }
		[Required(ErrorMessage = "Data zakończenia wizyty jest wymagana.")]
		public DateTime End { get; set; }

		[Required(ErrorMessage = "Wybranie pacjenta jest wymagane.")]
		public Patient Patient { get; set; }

	}
}
