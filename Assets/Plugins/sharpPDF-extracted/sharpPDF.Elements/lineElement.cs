using System;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF.Elements
{
	public sealed class lineElement : pdfElement
	{
		private int _coordX1;

		private int _coordY1;

		private pdfLineStyle _lineStyle;

		public lineElement(int X, int Y, int X1, int Y1)
			: this(X, Y, X1, Y1, 1, predefinedLineStyle.csNormal, pdfColor.Black)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, int newWidth)
			: this(X, Y, X1, Y1, newWidth, predefinedLineStyle.csNormal, pdfColor.Black)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, predefinedLineStyle newStyle)
			: this(X, Y, X1, Y1, 1, newStyle, pdfColor.Black)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, int newWidth, predefinedLineStyle newStyle)
			: this(X, Y, X1, Y1, newWidth, newStyle, pdfColor.Black)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, pdfColor newColor)
			: this(X, Y, X1, Y1, 1, predefinedLineStyle.csNormal, newColor)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, predefinedLineStyle newStyle, pdfColor newColor)
			: this(X, Y, X1, Y1, 1, newStyle, newColor)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, int newWidth, pdfColor newColor)
			: this(X, Y, X1, Y1, newWidth, predefinedLineStyle.csNormal, newColor)
		{
		}

		public lineElement(int X, int Y, int X1, int Y1, int newWidth, predefinedLineStyle newStyle, pdfColor newColor)
		{
			_coordX = X;
			_coordY = Y;
			_coordX1 = X1;
			_coordY1 = Y1;
			_strokeColor = newColor;
			_lineStyle = new pdfLineStyle(newWidth, newStyle);
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (_strokeColor.isColor())
			{
				stringBuilder2.Append(_strokeColor.rColor + " " + _strokeColor.gColor + " " + _strokeColor.bColor + " RG" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder2.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(_lineStyle.getText() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(_coordX.ToString() + " " + _coordY.ToString() + " m" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(_coordX1.ToString() + " " + _coordY1.ToString() + " l" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("S" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Length " + stringBuilder2.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(stringBuilder2.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2 = null;
			return stringBuilder.ToString();
		}

		public override object Clone()
		{
			return new lineElement(_coordX, _coordY, _coordX1, _coordY1, _lineStyle.width, _lineStyle.lineStyle, _strokeColor);
		}
	}
}
