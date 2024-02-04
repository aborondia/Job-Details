namespace sharpPDF
{
	public abstract class pdfPositionableObject
	{
		protected int _coordX;

		protected int _coordY;

		public int coordX
		{
			get
			{
				return _coordX;
			}
			set
			{
				_coordX = value;
			}
		}

		public int coordY
		{
			get
			{
				return _coordY;
			}
			set
			{
				_coordY = value;
			}
		}
	}
}
