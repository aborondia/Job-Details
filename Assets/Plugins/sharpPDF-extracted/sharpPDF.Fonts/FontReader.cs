using System;

namespace sharpPDF.Fonts
{
	internal abstract class FontReader : IDisposable
	{
		protected string _fontReference;

		public FontReader(string fontReference)
		{
			_fontReference = fontReference;
		}

		public abstract pdfFontDefinition getFontDefinition();

		public abstract void Dispose();
	}
}
