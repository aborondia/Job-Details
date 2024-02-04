using System;

namespace sharpPDF.Exceptions
{
	public class pdfBufferErrorException : pdfException
	{
		public pdfBufferErrorException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
