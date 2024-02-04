namespace sharpPDF.Exceptions
{
	public class pdfBadFontFileException : pdfException
	{
		public pdfBadFontFileException()
			: base("The font file is bad formatted", null)
		{
		}
	}
}
