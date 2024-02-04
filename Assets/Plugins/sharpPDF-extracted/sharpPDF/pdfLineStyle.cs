using System;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF
{
	internal class pdfLineStyle : IWritable
	{
		private predefinedLineStyle _lineStyle = predefinedLineStyle.csNormal;

		private int _width = 1;

		public int width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public predefinedLineStyle lineStyle
		{
			get
			{
				return _lineStyle;
			}
			set
			{
				_lineStyle = value;
			}
		}

		public pdfLineStyle()
		{
		}

		public pdfLineStyle(int newWidth)
		{
			_width = newWidth;
		}

		public pdfLineStyle(predefinedLineStyle newStyle)
		{
			_lineStyle = newStyle;
		}

		public pdfLineStyle(int newWidth, predefinedLineStyle newStyle)
		{
			_width = newWidth;
			_lineStyle = newStyle;
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_width.ToString() + " w" + Convert.ToChar(13) + Convert.ToChar(10));
			switch (_lineStyle)
			{
			case predefinedLineStyle.csOutlined:
				stringBuilder.Append("[4 4] 0 d" + Convert.ToChar(13) + Convert.ToChar(10));
				break;
			case predefinedLineStyle.csOutlinedSmall:
				stringBuilder.Append("[2 2] 0 d" + Convert.ToChar(13) + Convert.ToChar(10));
				break;
			case predefinedLineStyle.csOutlinedBig:
				stringBuilder.Append("[6 6] 0 d" + Convert.ToChar(13) + Convert.ToChar(10));
				break;
			}
			return stringBuilder.ToString();
		}
	}
}
