namespace sharpPDF.Exceptions
{
	public class pdfImageNotLoadedException : pdfException
	{
		public pdfImageNotLoadedException()
			: base("The image reference is not found inside the document!", null)
		{
		}
	}
}
