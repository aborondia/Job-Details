using System;
using System.Globalization;

namespace sharpPDF
{
	public class pdfColor : ICloneable
	{
		private string _rColor;

		private string _gColor;

		private string _bColor;

		public static readonly pdfColor NoColor = new pdfColor("", "", "");

		public static readonly pdfColor Black = new pdfColor("0", "0", "0");

		public static readonly pdfColor White = new pdfColor("1", "1", "1");

		public static readonly pdfColor Red = new pdfColor("1", "0", "0");

		public static readonly pdfColor LightRed = new pdfColor("1", ".75", ".75");

		public static readonly pdfColor DarkRed = new pdfColor(".5", "0", "0");

		public static readonly pdfColor Orange = new pdfColor("1", ".5", "0");

		public static readonly pdfColor LightOrange = new pdfColor("1", ".75", "0");

		public static readonly pdfColor DarkOrange = new pdfColor("1", ".25", "0");

		public static readonly pdfColor Yellow = new pdfColor("1", "1", ".25");

		public static readonly pdfColor LightYellow = new pdfColor("1", "1", ".75");

		public static readonly pdfColor DarkYellow = new pdfColor("1", "1", "0");

		public static readonly pdfColor Blue = new pdfColor("0", "0", "1");

		public static readonly pdfColor LightBlue = new pdfColor(".1", ".3", ".75");

		public static pdfColor DarkBlue = new pdfColor("0", "0", ".5");

		public static readonly pdfColor Green = new pdfColor("0", "1", "0");

		public static readonly pdfColor LightGreen = new pdfColor(".75", "1", ".75");

		public static readonly pdfColor DarkGreen = new pdfColor("0", ".5", "0");

		public static readonly pdfColor Cyan = new pdfColor("0", ".5", "1");

		public static readonly pdfColor LightCyan = new pdfColor(".2", ".8", "1");

		public static readonly pdfColor DarkCyan = new pdfColor("0", ".4", ".8");

		public static readonly pdfColor Purple = new pdfColor(".5", "0", "1");

		public static readonly pdfColor LightPurple = new pdfColor(".75", ".45", ".95");

		public static readonly pdfColor DarkPurple = new pdfColor(".4", ".1", ".5");

		public static readonly pdfColor Gray = new pdfColor(".5", ".5", ".5");

		public static readonly pdfColor LightGray = new pdfColor(".75", ".75", ".75");

		public static readonly pdfColor DarkGray = new pdfColor(".25", ".25", ".25");

		internal string rColor => _rColor;

		internal string gColor => _gColor;

		internal string bColor => _bColor;

		public pdfColor(string HEXColor)
		{
			_rColor = formatColorComponent(int.Parse(HEXColor.Substring(0, 2), NumberStyles.HexNumber));
			_gColor = formatColorComponent(int.Parse(HEXColor.Substring(2, 2), NumberStyles.HexNumber));
			_bColor = formatColorComponent(int.Parse(HEXColor.Substring(4, 2), NumberStyles.HexNumber));
		}

		public pdfColor(int rColor, int gColor, int bColor)
		{
			_rColor = formatColorComponent(rColor);
			_gColor = formatColorComponent(gColor);
			_bColor = formatColorComponent(bColor);
		}

		internal pdfColor(string rColor, string gColor, string bColor)
		{
			_rColor = rColor;
			_gColor = gColor;
			_bColor = bColor;
		}

		private string formatColorComponent(int colorValue)
		{
			int num = Convert.ToInt32(Math.Round(Convert.ToSingle(colorValue) / 255f * 100f));
			string text;
			if (num == 0 || num < 0)
			{
				text = "0";
			}
			else if (num < 100)
			{
				text = "." + num;
				if (text[text.Length - 1] == '0')
				{
					text = text.Substring(0, text.Length - 1);
				}
			}
			else
			{
				text = "1";
			}
			return text;
		}

		internal bool isColor()
		{
			if (_rColor != "" && _gColor != "")
			{
				return _bColor != "";
			}
			return false;
		}

		public object Clone()
		{
			return new pdfColor(_rColor, _gColor, _bColor);
		}
	}
}
