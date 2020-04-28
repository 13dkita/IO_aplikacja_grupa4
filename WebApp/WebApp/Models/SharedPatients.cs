using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
	public class SharedPatients
	{
		[Key]
		public int Id { get; set; }
		public Patient Patient { get; set; }

		public User Doctor { get; set; }
	}
}
