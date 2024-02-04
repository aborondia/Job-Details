using System;

namespace sharpPDF.Exceptions
{
	public class pdfWritingErrorException : pdfException
	{
		public pdfWritingErrorException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
