using System;

namespace sharpPDF.Exceptions
{
	public class pdfException : Exception
	{
		public pdfException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
