namespace sharpPDF.Exceptions
{
	public class pdfFontNotLoadedException : pdfException
	{
		public pdfFontNotLoadedException()
			: base("The font reference is not found inside the document!", null)
		{
		}
	}
}
