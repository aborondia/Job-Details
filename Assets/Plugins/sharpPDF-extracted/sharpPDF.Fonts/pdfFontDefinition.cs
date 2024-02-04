namespace sharpPDF.Fonts
{
	public class pdfFontDefinition
	{
		private string _fontName = "";

		private string _fullFontName = "";

		private string _familyName = "";

		private string _fontWeight = "";

		private int _italicAngle = 0;

		private bool _isFixedPitch = false;

		private string _characterSet = "";

		private int _underlinePosition = 0;

		private int _underlineThickness = 0;

		private string _encodingScheme = "";

		private int _capHeight = 0;

		private int _ascender = 0;

		private int _descender = 0;

		private int _StdHW = 0;

		private int _StdVW = 0;

		private int[] _fontBBox = new int[4];

		private int _fontHeight = 0;

		private object[] _fontMetrics = new object[65536];

		public string fontName
		{
			get
			{
				return _fontName;
			}
			set
			{
				_fontName = value;
			}
		}

		public string fullFontName
		{
			get
			{
				return _fullFontName;
			}
			set
			{
				_fullFontName = value;
			}
		}

		public string familyName
		{
			get
			{
				return _familyName;
			}
			set
			{
				_familyName = value;
			}
		}

		public string fontWeight
		{
			get
			{
				return _fontWeight;
			}
			set
			{
				_fontWeight = value;
			}
		}

		public bool isFixedPitch
		{
			get
			{
				return _isFixedPitch;
			}
			set
			{
				_isFixedPitch = value;
			}
		}

		public int italicAngle
		{
			get
			{
				return _italicAngle;
			}
			set
			{
				_italicAngle = value;
			}
		}

		public string characterSet
		{
			get
			{
				return _characterSet;
			}
			set
			{
				_characterSet = value;
			}
		}

		public int underlinePosition
		{
			get
			{
				return _underlinePosition;
			}
			set
			{
				_underlinePosition = value;
			}
		}

		public int underlineThickness
		{
			get
			{
				return _underlineThickness;
			}
			set
			{
				_underlineThickness = value;
			}
		}

		public string encodingScheme
		{
			get
			{
				return _encodingScheme;
			}
			set
			{
				_encodingScheme = value;
			}
		}

		public int capHeight
		{
			get
			{
				return _capHeight;
			}
			set
			{
				_capHeight = value;
			}
		}

		public int ascender
		{
			get
			{
				return _ascender;
			}
			set
			{
				_ascender = value;
			}
		}

		public int descender
		{
			get
			{
				return _descender;
			}
			set
			{
				_descender = value;
			}
		}

		public int StdHW
		{
			get
			{
				return _StdHW;
			}
			set
			{
				_StdHW = value;
			}
		}

		public int StdVW
		{
			get
			{
				return _StdVW;
			}
			set
			{
				_StdVW = value;
			}
		}

		public int[] fontBBox
		{
			get
			{
				return _fontBBox;
			}
			set
			{
				_fontBBox = value;
			}
		}

		public int fontHeight
		{
			get
			{
				return _fontHeight;
			}
			set
			{
				_fontHeight = value;
			}
		}

		public object[] fontMetrics
		{
			get
			{
				return _fontMetrics;
			}
			set
			{
				_fontMetrics = value;
			}
		}

		public bool validateFontDefinition()
		{
			return true;
		}
	}
}
