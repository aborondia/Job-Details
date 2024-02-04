namespace sharpPDF.Exceptions
{
	public class pdfBadRowHeightException : pdfException
	{
		public pdfBadRowHeightException()
			: base("The height of the row exceed the maximum height", null)
		{
		}
	}
}
