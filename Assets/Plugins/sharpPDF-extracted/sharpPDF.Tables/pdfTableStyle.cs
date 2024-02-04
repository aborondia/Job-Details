using sharpPDF.Fonts;

namespace sharpPDF.Tables
{
	public class pdfTableStyle
	{
		protected pdfAbstractFont _fontReference;

		protected int _fontSize;

		protected pdfColor _fontColor;

		protected pdfColor _bgColor;

		public pdfAbstractFont fontReference => _fontReference;

		public int fontSize => _fontSize;

		public pdfColor fontColor => _fontColor;

		public pdfColor bgColor => _bgColor;

		public pdfTableStyle(pdfAbstractFont fontReference, int fontSize, pdfColor fontColor, pdfColor bgColor)
		{
			_fontReference = fontReference;
			_fontSize = fontSize;
			_fontColor = fontColor;
			_bgColor = bgColor;
		}
	}
}
