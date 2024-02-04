using System;
using System.Collections;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF.Elements
{
	public sealed class annotationElement : pdfElement
	{
		private string _content;

		private predefinedAnnotationStyle _style;

		private ArrayList _styleNames = new ArrayList();

		private bool _open = false;

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

		public pdfColor color
		{
			get
			{
				return _strokeColor;
			}
			set
			{
				_strokeColor = value;
			}
		}

		public predefinedAnnotationStyle style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		internal void initializeStyleNames()
		{
			_styleNames.Insert(0, "Note");
			_styleNames.Insert(1, "Comment");
			_styleNames.Insert(2, "Key");
			_styleNames.Insert(3, "Note");
			_styleNames.Insert(4, "Help");
			_styleNames.Insert(5, "NewParagraph");
			_styleNames.Insert(6, "Paragraph");
			_styleNames.Insert(7, "Insert");
		}

		public annotationElement(string newContent, int newCoordX, int newCoordY, pdfColor newColor, predefinedAnnotationStyle newStyle)
			: this(newContent, newCoordX, newCoordY, newColor, newStyle, open: false)
		{
		}

		public annotationElement(string newContent, int newCoordX, int newCoordY, pdfColor newColor, predefinedAnnotationStyle newStyle, bool open)
		{
			_content = newContent;
			_coordX = newCoordX;
			_coordY = newCoordY;
			_strokeColor = newColor;
			_style = newStyle;
			_open = open;
			_height = 0;
			_width = 0;
			initializeStyleNames();
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Annot" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Subtype /Text" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Rect [" + _coordX.ToString() + " 0 0 " + _coordY.ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Contents (" + _content + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_open)
			{
				stringBuilder.Append("/Open true" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_strokeColor.isColor())
			{
				stringBuilder.Append("/C [" + _strokeColor.rColor + " " + _strokeColor.gColor + " " + _strokeColor.bColor + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_style != 0)
			{
				stringBuilder.Append(string.Concat("/Name /", _styleNames[(int)_style], Convert.ToChar(13), Convert.ToChar(10)));
			}
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public override object Clone()
		{
			return new annotationElement((string)_content.Clone(), _coordX, _coordY, (pdfColor)_strokeColor.Clone(), _style);
		}
	}
}
