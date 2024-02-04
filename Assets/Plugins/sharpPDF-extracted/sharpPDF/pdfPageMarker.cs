using System.Text;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;

namespace sharpPDF
{
	public class pdfPageMarker
	{
		private predefinedMarkerStyle _style = predefinedMarkerStyle.csArabic;

		private int _coordX;

		private int _coordY;

		private pdfAbstractFont _fontType;

		private int _fontSize;

		private pdfColor _fontColor;

		private string _pattern;

		public int coordX
		{
			get
			{
				return _coordX;
			}
			set
			{
				_coordX = value;
			}
		}

		public int coordY
		{
			get
			{
				return _coordY;
			}
			set
			{
				_coordY = value;
			}
		}

		public pdfAbstractFont fontType
		{
			get
			{
				return _fontType;
			}
			set
			{
				_fontType = value;
			}
		}

		public int fontSize
		{
			get
			{
				return _fontSize;
			}
			set
			{
				_fontSize = value;
			}
		}

		public pdfColor fontColor
		{
			get
			{
				return _fontColor;
			}
			set
			{
				_fontColor = value;
			}
		}

		public string pattern
		{
			get
			{
				return _pattern;
			}
			set
			{
				_pattern = value;
			}
		}

		public pdfPageMarker(int coordX, int coordY, predefinedMarkerStyle style, pdfAbstractFont fontReference, int fontSize)
			: this(coordX, coordY, style, fontReference, fontSize, pdfColor.Black, "Page #n# Of #N#")
		{
		}

		public pdfPageMarker(int coordX, int coordY, predefinedMarkerStyle style, pdfAbstractFont fontReference, int fontSize, pdfColor fontColor)
			: this(coordX, coordY, style, fontReference, fontSize, fontColor, "Page #n# Of #N#")
		{
		}

		public pdfPageMarker(int coordX, int coordY, predefinedMarkerStyle style, pdfAbstractFont fontReference, int fontSize, string pattern)
			: this(coordX, coordY, style, fontReference, fontSize, pdfColor.Black, pattern)
		{
		}

		public pdfPageMarker(int coordX, int coordY, predefinedMarkerStyle style, pdfAbstractFont fontReference, int fontSize, pdfColor fontColor, string pattern)
		{
			_style = style;
			_coordX = coordX;
			_coordY = coordY;
			_fontType = fontReference;
			_fontSize = fontSize;
			_fontColor = fontColor;
			_pattern = pattern;
		}

		public string getMarker(int pgIndex, int pgNum)
		{
			string newValue;
			string newValue2;
			switch (_style)
			{
			default:
				newValue = pgIndex.ToString();
				newValue2 = pgNum.ToString();
				break;
			case predefinedMarkerStyle.csRoman:
				newValue = arabicToRoman(pgIndex);
				newValue2 = arabicToRoman(pgNum);
				break;
			}
			return _pattern.Replace("#n#", newValue).Replace("#N#", newValue2);
		}

		private string arabicToRoman(int arabic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (arabic - 1000000 >= 0)
			{
				stringBuilder.Append("m");
				arabic -= 1000000;
			}
			while (arabic - 900000 >= 0)
			{
				stringBuilder.Append("cm");
				arabic -= 900000;
			}
			while (arabic - 500000 >= 0)
			{
				stringBuilder.Append("d");
				arabic -= 500000;
			}
			while (arabic - 400000 >= 0)
			{
				stringBuilder.Append("cd");
				arabic -= 400000;
			}
			while (arabic - 100000 >= 0)
			{
				stringBuilder.Append("c");
				arabic -= 100000;
			}
			while (arabic - 90000 >= 0)
			{
				stringBuilder.Append("xc");
				arabic -= 90000;
			}
			while (arabic - 50000 >= 0)
			{
				stringBuilder.Append("l");
				arabic -= 50000;
			}
			while (arabic - 40000 >= 0)
			{
				stringBuilder.Append("xl");
				arabic -= 40000;
			}
			while (arabic - 10000 >= 0)
			{
				stringBuilder.Append("x");
				arabic -= 10000;
			}
			while (arabic - 9000 >= 0)
			{
				stringBuilder.Append("Mx");
				arabic -= 9000;
			}
			while (arabic - 5000 >= 0)
			{
				stringBuilder.Append("v");
				arabic -= 5000;
			}
			while (arabic - 4000 >= 0)
			{
				stringBuilder.Append("Mv");
				arabic -= 4000;
			}
			while (arabic - 1000 >= 0)
			{
				stringBuilder.Append("M");
				arabic -= 1000;
			}
			while (arabic - 900 >= 0)
			{
				stringBuilder.Append("CM");
				arabic -= 900;
			}
			while (arabic - 500 >= 0)
			{
				stringBuilder.Append("D");
				arabic -= 500;
			}
			while (arabic - 400 >= 0)
			{
				stringBuilder.Append("CD");
				arabic -= 400;
			}
			while (arabic - 100 >= 0)
			{
				stringBuilder.Append("C");
				arabic -= 100;
			}
			while (arabic - 90 >= 0)
			{
				stringBuilder.Append("XC");
				arabic -= 90;
			}
			while (arabic - 50 >= 0)
			{
				stringBuilder.Append("L");
				arabic -= 50;
			}
			while (arabic - 40 >= 0)
			{
				stringBuilder.Append("XL");
				arabic -= 40;
			}
			while (arabic - 10 >= 0)
			{
				stringBuilder.Append("X");
				arabic -= 10;
			}
			while (arabic - 9 >= 0)
			{
				stringBuilder.Append("IX");
				arabic -= 9;
			}
			while (arabic - 5 >= 0)
			{
				stringBuilder.Append("V");
				arabic -= 5;
			}
			while (arabic - 4 >= 0)
			{
				stringBuilder.Append("IV");
				arabic -= 4;
			}
			while (arabic - 1 >= 0)
			{
				stringBuilder.Append("I");
				arabic--;
			}
			return stringBuilder.ToString();
		}
	}
}
