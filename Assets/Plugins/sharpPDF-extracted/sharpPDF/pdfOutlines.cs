using System;
using System.Collections;
using System.Text;
using sharpPDF.Bookmarks;

namespace sharpPDF
{
	internal class pdfOutlines : IWritable
	{
		private int _objectIDOutlines;

		private int _childIDFirst = 0;

		private int _childIDLast = 0;

		private int _childCount = 0;

		private ArrayList _BookmarkRoot = new ArrayList();

		public int objectIDOutlines
		{
			get
			{
				return _objectIDOutlines;
			}
			set
			{
				_objectIDOutlines = value;
			}
		}

		public int initializeOutlines(int counterID)
		{
			if (_BookmarkRoot.Count > 0)
			{
				initializeBookmarks(counterID, _BookmarkRoot, _objectIDOutlines);
				_childIDFirst = ((pdfBookmarkNode)_BookmarkRoot[0]).ObjectID;
				_childIDLast = ((pdfBookmarkNode)_BookmarkRoot[_BookmarkRoot.Count - 1]).ObjectID;
				counterID += _childCount;
			}
			else
			{
				_childCount = 0;
				_childIDFirst = 0;
				_childIDLast = 0;
			}
			return counterID;
		}

		public void addBookmark(pdfBookmarkNode Bookmark)
		{
			_BookmarkRoot.Add(Bookmark);
		}

		private int initializeBookmarks(int CounterID, ArrayList StartPoint, int FatherID)
		{
			int num = 0;
			if (StartPoint.Count > 0)
			{
				for (int i = 0; i < StartPoint.Count; i++)
				{
					pdfBookmarkNode pdfBookmarkNode = (pdfBookmarkNode)StartPoint[i];
					pdfBookmarkNode.ObjectID = CounterID + _childCount;
					pdfBookmarkNode.parent = FatherID;
					num++;
					_childCount++;
					pdfBookmarkNode.childCount = initializeBookmarks(CounterID, pdfBookmarkNode.Childs, pdfBookmarkNode.ObjectID);
					if (pdfBookmarkNode.childCount > 0)
					{
						pdfBookmarkNode.first = pdfBookmarkNode.getFirstChild().ObjectID;
						pdfBookmarkNode.last = pdfBookmarkNode.getLastChild().ObjectID;
					}
					if (StartPoint.Count > 1)
					{
						if (i > 0)
						{
							pdfBookmarkNode.prev = ((pdfBookmarkNode)StartPoint[i - 1]).ObjectID;
						}
						if (i < StartPoint.Count - 1)
						{
							pdfBookmarkNode.next = CounterID + _childCount;
						}
					}
					num += pdfBookmarkNode.childCount;
				}
			}
			return num;
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_objectIDOutlines.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Outlines" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_childCount != 0)
			{
				stringBuilder.Append("/First " + _childIDFirst.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("/Last " + _childIDLast.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
				stringBuilder.Append("/Count " + _childCount.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			}
			else
			{
				stringBuilder.Append("/Count 0" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		private ArrayList getNodes(ArrayList StartPoint)
		{
			ArrayList arrayList = new ArrayList();
			if (StartPoint.Count > 0)
			{
				arrayList.AddRange(StartPoint);
				foreach (pdfBookmarkNode item in StartPoint)
				{
					arrayList.AddRange(getNodes(item.Childs));
				}
			}
			return arrayList;
		}

		public ArrayList getBookmarks()
		{
			ArrayList nodes = getNodes(_BookmarkRoot);
			nodes.Sort();
			return nodes;
		}
	}
}
