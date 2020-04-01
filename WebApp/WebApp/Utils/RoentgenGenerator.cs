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
			Bitmap bmp = new Bitmap("Roentgen_Images/roentgen1.bmp");
			ImageConverter converter = new ImageConverter();
			return (byte[]) converter.ConvertTo(bmp, typeof(byte[]));
		}
	}
}
