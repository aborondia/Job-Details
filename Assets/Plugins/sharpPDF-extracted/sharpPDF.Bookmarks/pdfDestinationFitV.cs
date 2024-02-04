namespace sharpPDF.Bookmarks
{
	internal class pdfDestinationFitV : IPdfDestination
	{
		private int _left;

		public pdfDestinationFitV(int left)
		{
			_left = left;
		}

		public string getDestinationValue()
		{
			return "/FitV " + _left;
		}
	}
}
