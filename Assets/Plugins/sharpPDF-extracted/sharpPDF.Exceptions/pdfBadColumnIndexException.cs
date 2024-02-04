namespace sharpPDF.Exceptions
{
	public class pdfBadColumnIndexException : pdfException
	{
		public pdfBadColumnIndexException()
			: base("The columnd index does not exist", null)
		{
		}
	}
}
