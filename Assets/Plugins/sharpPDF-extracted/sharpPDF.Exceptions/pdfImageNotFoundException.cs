using System;

namespace sharpPDF.Exceptions
{
	public class pdfImageNotFoundException : pdfException
	{
		public pdfImageNotFoundException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
