using System;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF.Elements
{
	public sealed class circleElement : pdfElement
	{
		private const float kappa = 0.5522848f;

		private int _ray;

		private pdfLineStyle _lineStyle;

		public circleElement(int X, int Y, int Ray, pdfColor strokeColor, pdfColor fillColor)
			: this(X, Y, Ray, strokeColor, fillColor, 1, predefinedLineStyle.csNormal)
		{
		}

		public circleElement(int X, int Y, int Ray, pdfColor strokeColor, pdfColor fillColor, int newWidth)
			: this(X, Y, Ray, strokeColor, fillColor, newWidth, predefinedLineStyle.csNormal)
		{
		}

		public circleElement(int X, int Y, int Ray, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle newStyle)
			: this(X, Y, Ray, strokeColor, fillColor, 1, newStyle)
		{
		}

		public circleElement(int X, int Y, int Ray, pdfColor strokeColor, pdfColor fillColor, int newWidth, predefinedLineStyle newStyle)
		{
			_coordX = X;
			_coordY = Y;
			_ray = Ray;
			_strokeColor = strokeColor;
			_fillColor = fillColor;
			_lineStyle = new pdfLineStyle(newWidth, newStyle);
			_height = Ray * 2 + Convert.ToInt32(Math.Round((double)(newWidth / 2)));
			_width = _height;
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
			float num = (float)_ray * 0.5522848f;
			stringBuilder2.Append(_coordX.ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " m" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(Math.Round((float)_coordX + num, 0).ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " " + Math.Round((float)(_coordX + _ray), 0).ToString() + " " + Math.Round((float)_coordY + num, 0).ToString() + " " + Math.Round((float)(_coordX + _ray), 0).ToString() + " " + _coordY.ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(Math.Round((float)(_coordX + _ray), 0).ToString() + " " + Math.Round((float)_coordY - num, 0).ToString() + " " + Math.Round((float)_coordX + num, 0).ToString() + " " + Math.Round((float)(_coordY - _ray), 0).ToString() + " " + _coordX.ToString() + " " + Math.Round((float)(_coordY - _ray), 0).ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(Math.Round((float)_coordX - num, 0).ToString() + " " + Math.Round((float)(_coordY - _ray), 0).ToString() + " " + Math.Round((float)(_coordX - _ray), 0).ToString() + " " + Math.Round((float)_coordY - num, 0).ToString() + " " + Math.Round((float)(_coordX - _ray), 0).ToString() + " " + _coordY.ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(Math.Round((float)(_coordX - _ray), 0).ToString() + " " + Math.Round((float)_coordY + num, 0).ToString() + " " + Math.Round((float)_coordX - num, 0).ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " " + _coordX.ToString() + " " + Math.Round((float)(_coordY + _ray), 0).ToString() + " c" + Convert.ToChar(13) + Convert.ToChar(10));
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
			return new circleElement(_coordX, _coordY, _ray, (pdfColor)_strokeColor.Clone(), (pdfColor)_fillColor.Clone(), _lineStyle.width, _lineStyle.lineStyle);
		}
	}
}
