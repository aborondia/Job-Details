using System;
using System.Text;
using sharpPDF.Fonts;

namespace sharpPDF.Elements
{
	public sealed class textElement : pdfElement
	{
		private string _content;

		private int _fontSize;

		private pdfAbstractFont _fontType;

		public string content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
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

		public pdfAbstractFont fontType
		{
			get
			{
				return fontType;
			}
			set
			{
				_fontType = value;
			}
		}

		public textElement(string newContent, int newFontSize, pdfAbstractFont newFontType, int newCoordX, int newCoordY)
			: this(newContent, newFontSize, newFontType, newCoordX, newCoordY, pdfColor.Black)
		{
		}

		public textElement(string newContent, int newFontSize, pdfAbstractFont newFontType, int newCoordX, int newCoordY, pdfColor newStrokeColor)
		{
			_content = newContent;
			_fontSize = newFontSize;
			_fontType = newFontType;
			_coordX = newCoordX;
			_coordY = newCoordY;
			_strokeColor = newStrokeColor;
			_height = newFontType.fontDefinition.fontHeight * newFontSize;
			_width = newFontType.getWordWidth(newContent, newFontSize);
			if (newFontType is pdfTrueTypeFont)
			{
				((pdfTrueTypeFont)newFontType).addCharacters(newContent);
			}
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Filter [/ASCIIHexDecode]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("BT" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/F" + _fontType.fontNumber.ToString() + " " + _fontSize.ToString() + " Tf" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_strokeColor.isColor())
			{
				stringBuilder2.Append(_strokeColor.rColor + " " + _strokeColor.gColor + " " + _strokeColor.bColor + " rg" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder2.Append(_coordX.ToString() + " " + _coordY.ToString() + " Td" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_fontType is pdfTrueTypeFont)
			{
				stringBuilder2.Append("(" + textAdapter.checkText(_fontType.encodeText(_content)) + ") Tj" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			else
			{
				stringBuilder2.Append("(" + _fontType.encodeText(textAdapter.checkText(_content)) + ") Tj" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder2.Append("ET" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("Q");
			stringBuilder.Append("/Length " + (stringBuilder2.Length * 2 + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(textAdapter.HEXFormatter(stringBuilder2.ToString()) + ">" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2 = null;
			return stringBuilder.ToString();
		}

		public override object Clone()
		{
			return new textElement((string)_content.Clone(), _fontSize, _fontType, _coordX, _coordY, (pdfColor)_strokeColor.Clone());
		}
	}
}
