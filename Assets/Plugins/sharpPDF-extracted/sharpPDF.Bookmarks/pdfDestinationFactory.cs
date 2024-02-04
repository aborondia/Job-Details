namespace sharpPDF.Bookmarks
{
	public abstract class pdfDestinationFactory
	{
		public static IPdfDestination createPdfDestinationXYZ(int left, int top, int zoom)
		{
			return new pdfDestinationXYZ(left, top, zoom);
		}

		public static IPdfDestination createPdfDestinationFit()
		{
			return new pdfDestinationFit();
		}

		public static IPdfDestination createPdfDestinationFitH(int top)
		{
			return new pdfDestinationFitH(top);
		}

		public static IPdfDestination createPdfDestinationFitV(int left)
		{
			return new pdfDestinationFitV(left);
		}

		public static IPdfDestination createPdfDestinationFitR(int left, int top, int bottom, int right)
		{
			return new pdfDestinationFitR(left, top, bottom, right);
		}
	}
}
