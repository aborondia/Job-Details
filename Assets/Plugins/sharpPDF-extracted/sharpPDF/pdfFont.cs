using System;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF
{
	public class pdfFont : IWritable
	{
		private predefinedFont _fontStyle;

		private int _objectID;

		private int _fontNumber;

		public predefinedFont fontStyle => _fontStyle;

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

		internal pdfFont(predefinedFont newFontStyle, int newFontNumber)
		{
			_fontStyle = newFontStyle;
			_fontNumber = newFontNumber;
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Font" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Subtype /Type1" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Name /F" + _fontNumber.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/BaseFont /" + getFontName(_fontStyle) + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Encoding /WinAnsiEncoding" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public static string getFontName(predefinedFont fontType)
		{
			return fontType switch
			{
				predefinedFont.csHelvetica => "Helvetica", 
				predefinedFont.csHelveticaBold => "Helvetica-Bold", 
				predefinedFont.csHelveticaOblique => "Helvetica-Oblique", 
				predefinedFont.csHelvetivaBoldOblique => "Helvetica-BoldOblique", 
				predefinedFont.csCourier => "Courier", 
				predefinedFont.csCourierBold => "Courier-Bold", 
				predefinedFont.csCourierOblique => "Courier-Oblique", 
				predefinedFont.csCourierBoldOblique => "Courier-BoldOblique", 
				predefinedFont.csTimes => "Times-Roman", 
				predefinedFont.csTimesBold => "Times-Bold", 
				predefinedFont.csTimesOblique => "Times-Italic", 
				predefinedFont.csTimesBoldOblique => "Times-BoldItalic", 
				_ => "", 
			};
		}
	}
}
