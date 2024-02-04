using System;

namespace sharpPDF.Tables
{
	public class columnTableEventArgs : EventArgs
	{
		protected pdfTableColumn _column;

		public pdfTableColumn Column => _column;

		public columnTableEventArgs(pdfTableColumn column)
		{
			_column = column;
		}
	}
}
