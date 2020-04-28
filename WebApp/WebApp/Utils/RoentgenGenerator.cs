using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApp.Utils
{
	public static class RoentgenGenerator
	{
		public static byte[] LoadRandomImage()
		{
			var rnd = new Random();
			var files = Directory.GetFiles("Roentgen_Images", "*.jpg");
			Bitmap bmp = new Bitmap(files[rnd.Next(files.Length)]);
			ImageConverter converter = new ImageConverter();
			return (byte[]) converter.ConvertTo(bmp, typeof(byte[]));
		}
	}
}
