namespace sharpPDF.Exceptions
{
	public class pdfCMAPNotSupportedException : pdfException
	{
		public pdfCMAPNotSupportedException()
			: base("The CMAP of the font file is not supported", null)
		{
		}
	}
}
