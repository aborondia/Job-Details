using System;

namespace sharpPDF.Exceptions
{
	public class pdfImageIOException : pdfException
	{
		public pdfImageIOException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
