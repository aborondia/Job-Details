using System;
using System.Text;
using sharpPDF.Fonts;

namespace sharpPDF.Elements
{
	public sealed class paragraphLine : IWritable, ICloneable
	{
		private string _strLine;

		private int _lineLeftMargin;

		private int _lineTopMargin;

		private pdfAbstractFont _fontType;

		public int LineLeftMargin => _lineLeftMargin;

		public int LineTopMargin => _lineTopMargin;

		public string StrLine => _strLine;

		public paragraphLine(string strLine, int lineTopMargin, int lineLeftMargin, pdfAbstractFont fontType)
		{
			_strLine = strLine;
			_lineTopMargin = lineTopMargin;
			_lineLeftMargin = lineLeftMargin;
			_fontType = fontType;
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_lineLeftMargin.ToString() + " -" + _lineTopMargin.ToString() + " Td" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_fontType is pdfTrueTypeFont)
			{
				stringBuilder.Append("(" + textAdapter.checkText(_fontType.encodeText(_strLine)) + ") Tj" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			else
			{
				stringBuilder.Append("(" + _fontType.encodeText(textAdapter.checkText(_strLine)) + ") Tj" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append("-" + _lineLeftMargin.ToString().Replace(",", ".") + " 0 Td" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public object Clone()
		{
			return new paragraphLine((string)_strLine.Clone(), _lineTopMargin, _lineLeftMargin, _fontType);
		}
	}
}
