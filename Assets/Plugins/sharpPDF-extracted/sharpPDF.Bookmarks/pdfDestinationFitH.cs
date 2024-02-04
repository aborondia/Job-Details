namespace sharpPDF.Bookmarks
{
	internal class pdfDestinationFitH : IPdfDestination
	{
		private int _top;

		public pdfDestinationFitH(int top)
		{
			_top = top;
		}

		public string getDestinationValue()
		{
			return "/FitH " + _top;
		}
	}
}
