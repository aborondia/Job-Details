namespace sharpPDF.Bookmarks
{
	internal class pdfDestinationFitR : IPdfDestination
	{
		private int _left;

		private int _top;

		private int _bottom;

		private int _right;

		public pdfDestinationFitR(int left, int top, int bottom, int right)
		{
			_left = left;
			_top = top;
			_bottom = bottom;
			_right = right;
		}

		public string getDestinationValue()
		{
			return "/FitR " + _left + " " + _top + " " + _bottom + " " + _right;
		}
	}
}
