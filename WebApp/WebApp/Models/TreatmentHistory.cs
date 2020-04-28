using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
	public class TreatmentHistory
	{
		[Key]
		public int Id { get; set; }
		public Patient Patient { get; set; }

		public User Doctor { get; set; }
		public DateTime TreatmentDate
		{
			get
			{
				return this.treatmentDate.HasValue
					? this.treatmentDate.Value
					: DateTime.Now;
			}

			set { this.treatmentDate = value; }
		}

		private DateTime? treatmentDate = null;
	}
}
