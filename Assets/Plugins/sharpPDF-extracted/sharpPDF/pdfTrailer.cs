using System;
using System.Collections;
using System.Text;

namespace sharpPDF
{
	internal class pdfTrailer : IWritable
	{
		private int _lastObjectID;

		private ArrayList _objectOffsets;

		private long _xrefOffset;

		public long xrefOffset
		{
			get
			{
				return _xrefOffset;
			}
			set
			{
				_xrefOffset = value;
			}
		}

		public pdfTrailer(int lastObjectID)
		{
			_lastObjectID = lastObjectID;
			_objectOffsets = new ArrayList();
		}

		~pdfTrailer()
		{
			_objectOffsets = null;
		}

		public void addObject(string offset)
		{
			_objectOffsets.Add(new string('0', 10 - offset.Length) + offset);
		}

		public string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("xref" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("0 " + (_lastObjectID + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("0000000000 65535 f" + Convert.ToChar(13) + Convert.ToChar(10));
			foreach (object objectOffset in _objectOffsets)
			{
				stringBuilder.Append(objectOffset.ToString() + " 00000 n" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append("trailer" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Size " + (_lastObjectID + 1).ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Root 1 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Info 2 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("startxref" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(_xrefOffset.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("%%EOF");
			return stringBuilder.ToString();
		}
	}
}
