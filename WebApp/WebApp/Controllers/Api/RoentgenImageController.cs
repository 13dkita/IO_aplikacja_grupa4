using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoentgenImageController : ControllerBase
    {
	    private readonly ApplicationDbContext _db;

		public RoentgenImageController(ApplicationDbContext db)
		{
			_db = db;
		}

	    [HttpGet("{patientId:int}")]
	    public IActionResult GetProcessedImage(int patientId)
	    {
		    Patient foundPatient = _db.Patient.Where(p => p.Id == patientId).FirstOrDefault();

		    if (foundPatient == null)
			    return NotFound();
		    else
		    {
			    byte[] processedPhoto = ConvertGrayscaleImageToColor(foundPatient.RoentgenPhoto);

			    var base64Image = Convert.ToBase64String(processedPhoto);
			    var src = string.Format("data:/image/jpg;base64,{0}", base64Image);

				return Ok(src);
		    }
	    }

	    private byte[] ConvertGrayscaleImageToColor(byte[] pixels)
	    {
		    Bitmap bmp;
		    using (var ms = new MemoryStream(pixels))
		    {
			    bmp = new Bitmap(ms);

			    for (int x = 0; x < bmp.Width; x++)
			    {
				    for (int y = 0; y < bmp.Height; y++)
				    {
					    double color = bmp.GetPixel(x, y).R;
						bmp.SetPixel(x, y, GetColor((color) / 255));
				    }
			    }

				ImageConverter imageConverter = new ImageConverter();
				pixels = (byte[])imageConverter.ConvertTo(bmp, typeof(byte[]));
		    }

		    return pixels;
	    }

	    private Color GetColor(double x)
	    {
		    int R = (int)Math.Round(4.5 * 255 * x * (x - 1.0d/3.0d) * (x - 2.0 /3.0d));
			int G = (int)Math.Round(13.5 * 255 * x * (x - 2.0d /3.0d) * (x - 1.0d));
			int B = (int)Math.Round(-4.5 * 255 * (x - 1.0d /3.0d) * (x - 2.0d /3.0d) * (x - 1.0d));
			int Y = (int)Math.Round(-13.5 * 255 * x * (x - 1.0d /3.0d) * (x - 1.0d));

			Color color = Color.FromArgb(
				(byte)Math.Clamp(R + Y, 0, 255),
				(byte)Math.Clamp(G + Y, 0, 255),
				(byte)Math.Clamp(B, 0, 255));

			return color;
	    }
    }
}