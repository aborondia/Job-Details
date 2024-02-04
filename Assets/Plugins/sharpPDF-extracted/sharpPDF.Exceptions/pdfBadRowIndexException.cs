namespace sharpPDF.Exceptions
{
	public class pdfBadRowIndexException : pdfException
	{
		public pdfBadRowIndexException()
			: base("The row index does not exist", null)
		{
		}
	}
}
