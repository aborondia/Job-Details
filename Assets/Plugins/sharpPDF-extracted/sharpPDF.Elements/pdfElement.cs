using System;

namespace sharpPDF.Elements
{
	public abstract class pdfElement : pdfPositionableObject, IWritable, ICloneable
	{
		protected pdfColor _strokeColor = pdfColor.NoColor;

		protected pdfColor _fillColor = pdfColor.NoColor;

		protected int _objectID;

		protected int _height;

		protected int _width;

		public pdfColor strokeColor
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

		public pdfColor fillColor
		{
			get
			{
				return _fillColor;
			}
			set
			{
				_fillColor = value;
			}
		}

		public int height => _height;

		public int width => _width;

		internal int objectID
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

		public abstract string getText();

		public abstract object Clone();
	}
}
