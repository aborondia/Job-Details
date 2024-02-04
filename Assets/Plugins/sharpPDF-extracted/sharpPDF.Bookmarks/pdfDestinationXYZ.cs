namespace sharpPDF.Bookmarks
{
	internal class pdfDestinationXYZ : IPdfDestination
	{
		private int _left;

		private int _top;

		private int _zoom;

		public pdfDestinationXYZ(int left, int top, int zoom)
		{
			_left = left;
			_top = top;
			_zoom = zoom;
		}

		public string getDestinationValue()
		{
			return "/XYZ " + _left + " " + _top + " " + ((float)_zoom / 100f).ToString().Replace(",", ".");
		}

		private string getFormattedZoom()
		{
			if (_zoom >= 100)
			{
				string text = _zoom.ToString();
				return text.Substring(0, text.Length - 2) + "." + text.Substring(text.Length - 2, 2);
			}
			return "." + _zoom;
		}
	}
}
