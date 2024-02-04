namespace sharpPDF.Fonts
{
	internal class pdfCharacterMetric
	{
		private string _charName;

		private int _charIndex;

		private int _charWidth;

		public string charName => _charName;

		public int charIndex => _charIndex;

		public int charWidth => _charWidth;

		public pdfCharacterMetric(string charName, int charIndex, int charWidth)
		{
			_charName = charName;
			_charIndex = charIndex;
			_charWidth = charWidth;
		}
	}
}
