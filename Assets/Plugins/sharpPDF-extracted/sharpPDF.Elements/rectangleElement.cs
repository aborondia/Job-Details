using System;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF.Elements
{
	public sealed class rectangleElement : pdfElement
	{
		private int _coordX1;

		private int _coordY1;

		private pdfLineStyle _lineStyle;

		public rectangleElement(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor)
			: this(X, Y, X1, Y1, strokeColor, fillColor, 1, predefinedLineStyle.csNormal)
		{
		}

		public rectangleElement(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, int newWidth)
			: this(X, Y, X1, Y1, strokeColor, fillColor, newWidth, predefinedLineStyle.csNormal)
		{
		}

		public rectangleElement(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle newStyle)
			: this(X, Y, X1, Y1, strokeColor, fillColor, 1, newStyle)
		{
		}

		public rectangleElement(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, int newWidth, predefinedLineStyle newStyle)
		{
			_coordX = X;
			_coordY = Y;
			_coordX1 = X1;
			_coordY1 = Y1;
			_strokeColor = strokeColor;
			_fillColor = fillColor;
			_lineStyle = new pdfLineStyle(newWidth, newStyle);
			_width = Math.Max(X, X1) - Math.Min(X, X1) + Convert.ToInt32(Math.Round((double)(newWidth / 2)));
			_height = Math.Max(Y, Y1) - Math.Min(Y, Y1) + Convert.ToInt32(Math.Round((double)(newWidth / 2)));
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_strokeColor.isColor())
			{
				stringBuilder2.Append(_strokeColor.rColor + " " + _strokeColor.gColor + " " + _strokeColor.bColor + " RG" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_fillColor.isColor())
			{
				stringBuilder2.Append(_fillColor.rColor + " " + _fillColor.gColor + " " + _fillColor.bColor + " rg" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder2.Append(_lineStyle.getText() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(_coordX.ToString() + " " + _coordY.ToString() + " " + (_coordX1 - _coordX).ToString() + " " + (_coordY1 - _coordY).ToString() + " re" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("B" + Convert.ToChar(13) + Convert.ToChar(10));
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
			return new rectangleElement(_coordX, _coordY, _coordX1, _coordY1, (pdfColor)_strokeColor.Clone(), (pdfColor)_fillColor.Clone(), _lineStyle.width, _lineStyle.lineStyle);
		}
	}
}
