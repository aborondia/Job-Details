using sharpPDF.Collections;

namespace sharpPDF.PDFControls
{
	public abstract class pdfControl : pdfPositionableObject, ISeparable
	{
		protected int _height;

		protected int _width;

		protected pdfDocument _containerDocument;

		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public int Width
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

		public pdfControl(pdfDocument container)
		{
			_containerDocument = container;
		}

		public abstract elementList GetBasicElements();
	}
}
