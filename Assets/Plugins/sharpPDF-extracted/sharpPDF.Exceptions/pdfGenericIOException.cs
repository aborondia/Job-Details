using System;

namespace sharpPDF.Exceptions
{
	public class pdfGenericIOException : Exception
	{
		public pdfGenericIOException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
