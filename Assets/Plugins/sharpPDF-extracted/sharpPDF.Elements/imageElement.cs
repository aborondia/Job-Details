using System;
using System.Text;

namespace sharpPDF.Elements
{
	public sealed class imageElement : pdfElement
	{
		private pdfImageReference _ObjectXReference;

		public pdfImageReference ObjectXReference => _ObjectXReference;

		public imageElement(pdfImageReference imageReference, int newCoordX, int newCoordY)
		{
			_ObjectXReference = imageReference;
			_height = _ObjectXReference.height;
			_width = _ObjectXReference.width;
			_coordX = newCoordX;
			_coordY = newCoordY;
		}

		public imageElement(pdfImageReference imageReference, int newCoordX, int newCoordY, int newHeight, int newWidth)
		{
			_ObjectXReference = imageReference;
			_height = newHeight;
			_width = newWidth;
			_coordX = newCoordX;
			_coordY = newCoordY;
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("q" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(_width.ToString() + " 0 0 " + _height.ToString() + " " + _coordX.ToString() + " " + _coordY.ToString() + " cm" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/I" + _ObjectXReference.ObjectID.ToString() + " Do" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("Q" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Length " + stringBuilder2.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(stringBuilder2.ToString());
			stringBuilder.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2 = null;
			return stringBuilder.ToString();
		}

		public override object Clone()
		{
			return new imageElement(_ObjectXReference, _coordX, _coordY, _height, _width);
		}
	}
}
