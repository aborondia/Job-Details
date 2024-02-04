using sharpPDF.Enumerators;

namespace sharpPDF.Fonts
{
	public abstract class pdfAbstractFont : IWritable
	{
		protected int _objectID;

		protected int _fontNumber;

		protected pdfFontDefinition _fontDefinition;

		protected documentFontEncoding _encodingType;

		internal int objectID
		{
			get
			{
				return _objectID;
			}
			set
			{
				_objectID = value;
			}
		}

		internal int fontNumber => _fontNumber;

		internal pdfFontDefinition fontDefinition => _fontDefinition;

		internal documentFontEncoding encodingType => _encodingType;

		internal pdfAbstractFont(pdfFontDefinition fontDefinition, int fontNumber, documentFontEncoding encodingType)
		{
			_fontDefinition = fontDefinition;
			_fontNumber = fontNumber;
			_encodingType = encodingType;
		}

		public abstract string getText();

		public abstract string encodeText(string strText);

		public abstract string cleanText(string strText);

		public abstract int getWordWidth(string strWord, int fontSize);

		public abstract string cropWord(string strWord, int maxLengh, int fontSize);
	}
}
