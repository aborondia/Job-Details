using System;
using System.Collections;
using System.Text;

namespace sharpPDF.Bookmarks
{
	public class pdfBookmarkNode : IWritable, IComparable
	{
		private string _Title;

		private pdfPage _Page;

		private IPdfDestination _Destination;

		private bool _open;

		private int _ObjectID;

		private int _prev;

		private int _next;

		private int _first;

		private int _last;

		private int _parent;

		private int _childCount;

		private ArrayList _Childs = new ArrayList();

		internal string Title => _Title;

		internal pdfPage Page => _Page;

		internal IPdfDestination Destination => _Destination;

		internal bool open => _open;

		internal int ObjectID
		{
			get
			{
				return _ObjectID;
			}
			set
			{
				_ObjectID = value;
			}
		}

		internal int prev
		{
			get
			{
				return _prev;
			}
			set
			{
				_prev = value;
			}
		}

		internal int next
		{
			get
			{
				return _next;
			}
			set
			{
				_next = value;
			}
		}

		internal int first
		{
			get
			{
				return _first;
			}
			set
			{
				_first = value;
			}
		}

		internal int last
		{
			get
			{
				return _last;
			}
			set
			{
				_last = value;
			}
		}

		internal int parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		internal int childCount
		{
			get
			{
				return _childCount;
			}
			set
			{
				_childCount = value;
			}
		}

		internal ArrayList Childs => _Childs;

		public int CompareTo(object obj)
		{
			if (obj is pdfBookmarkNode)
			{
				pdfBookmarkNode pdfBookmarkNode2 = (pdfBookmarkNode)obj;
				return _ObjectID.CompareTo((object)pdfBookmarkNode2.ObjectID);
			}
			throw new ArgumentException("Object is not a pdfBookmarkNode");
		}

		internal pdfBookmarkNode getFirstChild()
		{
			if (_childCount > 0)
			{
				return (pdfBookmarkNode)_Childs[0];
			}
			return null;
		}

		internal pdfBookmarkNode getLastChild()
		{
			if (_childCount > 0)
			{
				return (pdfBookmarkNode)_Childs[_Childs.Count - 1];
			}
			return null;
		}

		public pdfBookmarkNode(string Title, pdfPage Page, bool openBookmark)
		{
			_Title = Title;
			_Page = Page;
			_Destination = null;
			_prev = 0;
			_next = 0;
			_first = 0;
			_last = 0;
			_parent = 0;
			_childCount = 0;
			_open = openBookmark;
		}

		public pdfBookmarkNode(string Title, pdfPage Page, bool openBookmark, IPdfDestination Destination)
		{
			_Title = Title;
			_Page = Page;
			_Destination = Destination;
			_prev = 0;
			_next = 0;
			_first = 0;
			_last = 0;
			_parent = 0;
			_childCount = 0;
			_open = openBookmark;
		}

		public void addChildNode(pdfBookmarkNode Child)
		{
			_Childs.Add(Child);
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_ObjectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Title (" + textAdapter.checkText(_Title) + ")" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_prev > 0)
			{
				stringBuilder.Append("/Prev " + _prev.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_next > 0)
			{
				stringBuilder.Append("/Next " + _next.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_parent > 0)
			{
				stringBuilder.Append("/Parent " + _parent.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			if (_childCount > 0)
			{
				stringBuilder.Append("/First " + _first.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("/Last " + _last.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
				if (_open)
				{
					stringBuilder.Append("/Count " + _childCount.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
				}
			}
			if (_Destination != null)
			{
				stringBuilder.Append("/Dest [" + _Page.objectID.ToString() + " 0 R " + _Destination.getDestinationValue() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}
	}
}
