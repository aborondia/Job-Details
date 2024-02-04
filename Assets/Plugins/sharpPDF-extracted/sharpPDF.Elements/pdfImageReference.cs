using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using sharpPDF.Exceptions;

namespace sharpPDF.Elements
{
	public sealed class pdfImageReference : IWritable
	{
		private int _ObjectID;

		private int _height;

		private int _width;

		private byte[] _content;

		internal int ObjectID
		{
			get
			{
				return _ObjectID;
			}
			set
			{
				_ObjectID = value;
			}
		}

		public int height => _height;

		public int width => _width;

		internal byte[] content => _content;

		internal pdfImageReference(string imageName)
		{
			try
			{
				Image image = Image.FromFile(imageName);
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, ImageFormat.Jpeg);
				_content = new byte[memoryStream.Length];
				_content = memoryStream.ToArray();
				image.Dispose();
				image = Image.FromStream(memoryStream);
				_height = image.Height;
				_width = image.Width;
				image.Dispose();
				memoryStream.Close();
				memoryStream = null;
				image = null;
			}
			catch (FileNotFoundException ex)
			{
				throw new pdfImageNotFoundException("Image " + imageName + " not found!", ex);
			}
			catch (IOException ex2)
			{
				throw new pdfImageIOException("Generic IO error on the image  " + imageName, ex2);
			}
		}

		internal pdfImageReference(Image myImage)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				myImage.Save(memoryStream, ImageFormat.Jpeg);
				_content = new byte[memoryStream.Length];
				_content = memoryStream.ToArray();
				myImage.Dispose();
				myImage = Image.FromStream(memoryStream);
				_height = myImage.Height;
				_width = myImage.Width;
				myImage.Dispose();
				memoryStream.Close();
				memoryStream = null;
				myImage = null;
			}
			catch (IOException ex)
			{
				throw new pdfImageIOException("Generic IO error on the image!", ex);
			}
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_ObjectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /XObject" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Subtype /Image" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Name /I" + _ObjectID.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Filter /DCTDecode" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Width " + _width.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Height " + _height.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/BitsPerComponent 8" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/ColorSpace /DeviceRGB" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Length " + _content.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}
	}
}
