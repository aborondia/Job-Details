using System;
using System.Collections;
using System.Text;
using sharpPDF.Enumerators;
using sharpPDF.Exceptions;
using sharpPDF.Fonts.TTF;

namespace sharpPDF.Fonts
{
	public sealed class pdfTrueTypeFont : pdfAbstractFont
	{
		private ArrayList _usedCharacters = new ArrayList();

		private int[] _glyphMapping = new int[65535];

		private string _fontReference;

		private int _descriptorObjectID;

		private int _toUnicodeObjectID;

		private int _descendantObjectID;

		private int _streamObjectID;

		private byte[] _subsetStream;

		internal int descriptorObjectID
		{
			get
			{
				return _descriptorObjectID;
			}
			set
			{
				_descriptorObjectID = value;
			}
		}

		internal int toUnicodeObjectID
		{
			get
			{
				return _toUnicodeObjectID;
			}
			set
			{
				_toUnicodeObjectID = value;
			}
		}

		internal int descendantObjectID
		{
			get
			{
				return _descendantObjectID;
			}
			set
			{
				_descendantObjectID = value;
			}
		}

		internal int streamObjectID
		{
			get
			{
				return _streamObjectID;
			}
			set
			{
				_streamObjectID = value;
			}
		}

		internal byte[] subsetStream => _subsetStream;

		internal pdfTrueTypeFont(pdfFontDefinition fontDefinition, int fontNumber, documentFontEncoding encodingType, string fontReference)
			: base(fontDefinition, fontNumber, encodingType)
		{
			_fontReference = fontReference;
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Font" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Subtype /Type0" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Name /F" + _fontNumber.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/BaseFont /" + _fontDefinition.fontName + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/DescendantFonts [" + _descendantObjectID.ToString() + " 0 R]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Encoding /Identity-H" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/ToUnicode " + _toUnicodeObjectID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public string getFontDescriptorText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_descriptorObjectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /FontDescriptor" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/FontName /" + _fontDefinition.fontName + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/StemV 80" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Descent " + _fontDefinition.descender.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Ascent " + _fontDefinition.ascender.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/ItalicAngle " + _fontDefinition.italicAngle.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/CapHeight " + _fontDefinition.capHeight.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Flags 32" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/FontFile2 " + _streamObjectID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/FontBBox [" + _fontDefinition.fontBBox[0].ToString() + " " + _fontDefinition.fontBBox[1].ToString() + " " + _fontDefinition.fontBBox[2].ToString() + " " + _fontDefinition.fontBBox[3].ToString() + "]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public string getFontDescendantText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_descendantObjectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Font" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/BaseFont /" + _fontDefinition.fontName + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/CIDSystemInfo <</Ordering (Identity)" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Registry (Adobe)" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Supplement 0" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/CIDToGIDMap /Identity" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/FontDescriptor " + _descriptorObjectID.ToString() + " 0 R" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Subtype /CIDFontType2" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/DW 1000" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_usedCharacters.Count > 0)
			{
				stringBuilder.Append("/W [");
				stringBuilder.Append(getMetric((char)_usedCharacters[0]).characterMapping + "[" + getMetric((char)_usedCharacters[0]).characterWidth);
				CharacterMetric characterMetric = getMetric((char)_usedCharacters[0]);
				for (int i = 1; i < _usedCharacters.Count; i++)
				{
					CharacterMetric metric = getMetric((char)_usedCharacters[i]);
					if (metric.characterMapping != characterMetric.characterMapping + 1)
					{
						stringBuilder.Append("]" + metric.characterMapping + "[" + metric.characterWidth);
					}
					else
					{
						stringBuilder.Append(" " + metric.characterWidth);
					}
					characterMetric = metric;
				}
				stringBuilder.Append("]]" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public string getToUnicodeText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("/CIDInit /ProcSet findresource begin" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("12 dict begin" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("begincmap" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/CIDSystemInfo" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("<< /Registry (Adobe)" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/Ordering (UCS)" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/Supplement 0" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(">> def" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/CMapName /Adobe-Identity-UCS def" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("/CMapType 2 def" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("1 begincodespacerange" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_usedCharacters.Count > 1)
			{
				stringBuilder2.Append("<" + getMetric((char)_usedCharacters[0]).characterMapping.ToString("X4").ToLower() + "><" + getMetric((char)_usedCharacters[_usedCharacters.Count - 1]).characterMapping.ToString("X4").ToLower() + ">" + Convert.ToChar(13) + Convert.ToChar(10));
			}
			else
			{
				stringBuilder2.Append("<>" + Convert.ToChar(10) + Convert.ToChar(13));
			}
			stringBuilder2.Append("endcodespacerange" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append(_usedCharacters.Count.ToString() + " beginbfrange" + Convert.ToChar(13) + Convert.ToChar(10));
			if (_usedCharacters.Count > 0)
			{
				foreach (object usedCharacter in _usedCharacters)
				{
					char c = (char)usedCharacter;
					object[] array = new object[9]
					{
						"<",
						getMetric(c).characterMapping.ToString("X4").ToLower(),
						"><",
						getMetric(c).characterMapping.ToString("X4").ToLower(),
						"><",
						null,
						null,
						null,
						null
					};
					object[] array2 = array;
					int num = c;
					array2[5] = num.ToString("X4").ToLower();
					array[6] = ">";
					array[7] = Convert.ToChar(13);
					array[8] = Convert.ToChar(10);
					stringBuilder2.Append(string.Concat(array));
				}
			}
			stringBuilder2.Append("endbfrange" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("endcmap" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("CMapName currentdict /CMap defineresource pop" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder2.Append("end end");
			stringBuilder.Append(_toUnicodeObjectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Filter [/ASCIIHexDecode]" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Length " + stringBuilder2.Length.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("stream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(textAdapter.HEXFormatter(stringBuilder2.ToString()) + ">" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endstream" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public string getStreamText()
		{
			createFontSubset();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_streamObjectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<</Length " + _subsetStream.Length.ToString() + ">>" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		private void createFontSubset()
		{
			ttfFontSubset ttfFontSubset = null;
			try
			{
				Hashtable hashtable = new Hashtable();
				for (int i = 0; i < _usedCharacters.Count; i++)
				{
					hashtable.Add(getMetric((char)_usedCharacters[i]).characterMapping, new int[3]
					{
						getMetric((char)_usedCharacters[i]).characterMapping,
						getMetric((char)_usedCharacters[i]).characterWidth,
						(char)_usedCharacters[i]
					});
				}
				ttfFontSubset = new ttfFontSubset(_fontReference, hashtable);
				_subsetStream = ttfFontSubset.getFontFileStream();
			}
			catch (Exception)
			{
				throw new pdfBadFontFileException();
			}
			finally
			{
				if (ttfFontSubset != null)
				{
					ttfFontSubset = null;
				}
			}
		}

		public override string encodeText(string strText)
		{
			char[] array = strText.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				CharacterMetric characterMetric = (CharacterMetric)_fontDefinition.fontMetrics[(uint)array[i]];
				if (characterMetric.characterMapping != 0)
				{
					characterMetric = (CharacterMetric)_fontDefinition.fontMetrics[(uint)array[i]];
					char c = (char)characterMetric.characterMapping;
					stringBuilder.Append((char)((int)c >> 8));
					stringBuilder.Append((char)(c & 0xFFu));
				}
			}
			return stringBuilder.ToString();
		}

		public void addCharacters(string myText)
		{
			char[] array = myText.ToCharArray();
			foreach (char c in array)
			{
				if (_usedCharacters.Contains(c))
				{
					continue;
				}
				CharacterMetric characterMetric = (CharacterMetric)_fontDefinition.fontMetrics[(uint)c];
				if (characterMetric.characterMapping == 0)
				{
					continue;
				}
				bool flag = false;
				int num = 0;
				while (num < _usedCharacters.Count && !flag)
				{
					if (getMetric(c).characterMapping < getMetric((char)_usedCharacters[num]).characterMapping)
					{
						flag = true;
					}
					else
					{
						num++;
					}
				}
				if (!flag)
				{
					_usedCharacters.Add(c);
				}
				else
				{
					_usedCharacters.Insert(num, c);
				}
			}
		}

		private CharacterMetric getMetric(char myChar)
		{
			return (CharacterMetric)_fontDefinition.fontMetrics[(uint)myChar];
		}

		public override string cleanText(string strText)
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = strText.ToCharArray();
			foreach (char c in array)
			{
				if (getMetric(c).characterMapping != 0)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public override int getWordWidth(string strWord, int fontSize)
		{
			double num = 0.0;
			char[] array = strWord.ToCharArray();
			foreach (char c in array)
			{
				double num2 = num;
				CharacterMetric characterMetric = (CharacterMetric)_fontDefinition.fontMetrics[(uint)c];
				num = num2 + (double)characterMetric.characterWidth;
			}
			return Convert.ToInt32(Math.Round(num * (double)fontSize / 1000.0));
		}

		public override string cropWord(string strWord, int maxLength, int fontSize)
		{
			StringBuilder stringBuilder = new StringBuilder();
			strWord = cleanText(strWord);
			int i = 0;
			if (getWordWidth(strWord, fontSize) <= maxLength || strWord.Length == 0)
			{
				return strWord;
			}
			for (; getWordWidth(stringBuilder.ToString() + strWord[i] + "...", fontSize) <= maxLength && i < strWord.Length; i++)
			{
				stringBuilder.Append(strWord[i]);
			}
			if (stringBuilder.Length == 0)
			{
				return "";
			}
			return stringBuilder.ToString() + "...";
		}
	}
}
