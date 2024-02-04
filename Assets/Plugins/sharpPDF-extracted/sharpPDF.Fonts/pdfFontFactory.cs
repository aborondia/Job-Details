using System;
using sharpPDF.Enumerators;
using sharpPDF.Fonts.AFM;
using sharpPDF.Fonts.TTF;

namespace sharpPDF.Fonts
{
	internal abstract class pdfFontFactory
	{
		private static readonly string[] predefinedFontName = new string[13]
		{
			"None", "Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique", "Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique", "Times-Roman",
			"Times-Bold", "Times-Italic", "Times-BoldItalic"
		};

		public static pdfAbstractFont getFontObject(string fontReference, int fontNumber, documentFontType fontType)
		{
			pdfAbstractFont pdfAbstractFont2 = null;
			FontReader fontReader = getFontReader(fontReference, fontType);
			pdfAbstractFont2 = fontType switch
			{
				documentFontType.csTrueTypeFont => new pdfTrueTypeFont(fontReader.getFontDefinition(), fontNumber, documentFontEncoding.csIdentityH, fontReference), 
				_ => new pdfPredefinedFont(fontReader.getFontDefinition(), fontNumber, documentFontEncoding.csWinAnsiEncoding), 
			};
			fontReader = null;
			return pdfAbstractFont2;
		}

		private static FontReader getFontReader(string fontReference, documentFontType fontType)
		{
			return fontType switch
			{
				documentFontType.csTrueTypeFont => new ttfFontReader(fontReference), 
				_ => new afmFontReader(fontReference), 
			};
		}

		public static string getPredefinedFontName(predefinedFont fontStyle)
		{
			return predefinedFontName[Convert.ToInt32(fontStyle)];
		}

		public static bool isPredefinedFont(string fontReference)
		{
			switch (fontReference)
			{
			default:
				return fontReference == "Times-BoldItalic";
			case "Helvetica":
			case "Helvetica-Bold":
			case "Helvetica-Oblique":
			case "Helvetica-BoldOblique":
			case "Courier":
			case "Courier-Bold":
			case "Courier-Oblique":
			case "Courier-BoldOblique":
			case "Times-Roman":
			case "Times-Bold":
			case "Times-Italic":
				return true;
			}
		}
	}
}
