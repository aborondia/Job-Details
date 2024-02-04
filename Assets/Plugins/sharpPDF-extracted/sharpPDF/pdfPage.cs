using System;
using System.Text;
using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;

namespace sharpPDF
{
	public class pdfPage : pdfBasePage, IWritable
	{
		private int _height;

		private int _width;

		private int _objectID;

		private int _pageTreeID;

		public int objectID
		{
			get
			{
				return _objectID;
			}
			set
			{
				_objectID = value;
			}
		}

		public int pageTreeID
		{
			get
			{
				return _pageTreeID;
			}
			set
			{
				_pageTreeID = value;
			}
		}

		public int height => _height;

		public int width => _width;

		internal pdfPage(pdfDocument containerDoc)
			: base(containerDoc)
		{
			_height = 792;
			_width = 612;
		}

		internal pdfPage(predefinedPageSize predefinedSize, pdfDocument containerDoc)
			: base(containerDoc)
		{
			switch (predefinedSize)
			{
			case predefinedPageSize.csSharpPDFFormat:
				_height = 792;
				_width = 612;
				break;
			case predefinedPageSize.csA1Page:
				_height = 2288;
				_width = 1655;
				break;
			case predefinedPageSize.csA2Page:
				_height = 1684;
				_width = 1191;
				break;
			case predefinedPageSize.csA3Page:
				_height = 1191;
				_width = 842;
				break;
			case predefinedPageSize.csA4Page:
				_height = 842;
				_width = 595;
				break;
			}
		}

		internal pdfPage(int newHeight, int newWidth, pdfDocument containerDoc)
			: base(containerDoc)
		{
			_height = newHeight;
			_width = newWidth;
		}

		~pdfPage()
		{
			_containerDoc = null;
			_elements = null;
		}

		private string addFonts()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (pdfAbstractFont value in _containerDoc._fonts.Values)
			{
				stringBuilder.Append("/F" + value.fontNumber + " " + value.objectID + " 0 R ");
			}
			return stringBuilder.ToString();
		}

		private string addImages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (pdfImageReference value in _containerDoc._images.Values)
			{
				stringBuilder.Append("/I" + value.ObjectID + " " + value.ObjectID + " 0 R ");
			}
			return stringBuilder.ToString();
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Page" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Parent " + _pageTreeID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Resources <<" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_containerDoc._fonts.Count > 0)
			{
				stringBuilder.Append("/Font <<" + addFonts() + ">>" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_containerDoc._images.Count > 0)
			{
				stringBuilder.Append("/XObject <<" + addImages() + ">>" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/MediaBox [0 0 " + _width + " " + _height + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/CropBox [0 0 " + _width + " " + _height + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Rotate 0" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/ProcSet [/PDF /Text /ImageC]" + Convert.ToChar(13) + Convert.ToChar(10));
			foreach (pdfElement element in _elements)
			{
				if (element is annotationElement)
				{
					stringBuilder3.Append(element.objectID + " 0 R ");
				}
				else
				{
					stringBuilder2.Append(element.objectID + " 0 R ");
				}
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder.Append("/Contents [" + stringBuilder2.ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (stringBuilder3.Length > 0)
			{
				stringBuilder.Append("/Annots [" + stringBuilder3.ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2 = null;
			return stringBuilder.ToString();
		}
	}
}
