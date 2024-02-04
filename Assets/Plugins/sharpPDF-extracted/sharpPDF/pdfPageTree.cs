using System;
using System.Collections;
using System.Text;

namespace sharpPDF
{
	internal class pdfPageTree : IWritable
	{
		private ArrayList _pages;

		private int _pageCount;

		private int _objectID;

		public int objectID
		{
			get
			{
				return _objectID;
			}
			set
			{
				_objectID = value;
			}
		}

		public pdfPageTree()
		{
			_pageCount = 0;
			_pages = new ArrayList();
		}

		public void addPage(int pageID)
		{
			_pages.Add(pageID);
			_pageCount++;
		}

		public string getText()
		{
			if (_pages.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("/Type /Pages" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("/Count " + _pages.Count.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("/Kids [");
				foreach (object page in _pages)
				{
					int num = (int)page;
					stringBuilder.Append(num + " 0 R ");
				}
				stringBuilder.Append("]" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
				return stringBuilder.ToString();
			}
			return null;
		}
	}
}
