using System;
using System.Text;

namespace sharpPDF
{
	internal class pdfInfo : IWritable
	{
		private int _objectIDInfo;

		private string _title;

		private string _author;

		public int objectIDInfo
		{
			get
			{
				return _objectIDInfo;
			}
			set
			{
				_objectIDInfo = value;
			}
		}

		public pdfInfo(string title, string author)
		{
			_title = title;
			_author = author;
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_objectIDInfo.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Title (" + _title + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Author (" + _author + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Creator (sharpPDF_2.0)" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/CreationDate (" + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Day.ToString() + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}
	}
}
