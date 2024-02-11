using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using sharpPDF.Exceptions;
using UnityEngine;
using UnityEngine.Networking;

namespace sharpPDF.Fonts.AFM
{
	internal class afmFontReader : FontReader
	{
		private char[] csDelimiterToken = new char[2] { ' ', '\t' };

		private char[] csSemicolonToken = new char[1] { ';' };

		private StreamReader _afmStream = null;
		private string fontsDirectory => Path.Combine(Application.streamingAssetsPath, "Fonts");

		public afmFontReader(string fontReference)
			: base(fontReference)
		{
			try
			{
				_afmStream = new StreamReader(Path.Combine(fontsDirectory, $"{fontReference}.afm"));
			}
			catch (Exception ex)
			{
				throw new pdfGenericIOException(ex.Message, ex);
			}
		}

		public afmFontReader(string fontName, byte[] fontReference)
			: base(fontName)
		{
			try
			{
				_afmStream = new StreamReader(new MemoryStream(fontReference));
			}
			catch (Exception ex)
			{
				throw new pdfGenericIOException(ex.Message, ex);
			}
		}

		public override pdfFontDefinition getFontDefinition()
		{
			pdfFontDefinition pdfFontDefinition = new pdfFontDefinition();
			string text = null;
			string[] array = null;
			pdfCharacterMetric pdfCharacterMetric = null;
			bool flag = false;
			try
			{
				for (text = _afmStream.ReadLine(); text != null; text = _afmStream.ReadLine())
				{
					array = text.Split(csDelimiterToken);
					switch (array[0])
					{
						case "FontName":
							pdfFontDefinition.fontName = array[1].Trim();
							break;
						case "FullName":
							pdfFontDefinition.fullFontName = array[1].Trim();
							break;
						case "FamilyName":
							pdfFontDefinition.familyName = array[1].Trim();
							break;
						case "Weight":
							pdfFontDefinition.fontWeight = array[1].Trim();
							break;
						case "ItalicAngle":
							pdfFontDefinition.italicAngle = int.Parse(array[1]);
							break;
						case "IsFixedPitch":
							pdfFontDefinition.isFixedPitch = bool.Parse(array[1]);
							break;
						case "CharacterSet":
							pdfFontDefinition.characterSet = array[1].Trim();
							break;
						case "FontBBox":
							pdfFontDefinition.fontBBox[0] = int.Parse(array[1]);
							pdfFontDefinition.fontBBox[1] = int.Parse(array[2]);
							pdfFontDefinition.fontBBox[2] = int.Parse(array[3]);
							pdfFontDefinition.fontBBox[3] = int.Parse(array[4]);
							pdfFontDefinition.fontHeight = Convert.ToInt32(Math.Round(((double)pdfFontDefinition.fontBBox[3] - (double)pdfFontDefinition.fontBBox[1]) / 1000.0));
							if (pdfFontDefinition.fontHeight == 0)
							{
								pdfFontDefinition.fontHeight = 1;
							}
							break;
						case "UnderlinePosition":
							pdfFontDefinition.underlinePosition = int.Parse(array[1]);
							break;
						case "UnderlineThickness":
							pdfFontDefinition.underlineThickness = int.Parse(array[1]);
							break;
						case "EncodingScheme":
							pdfFontDefinition.encodingScheme = array[1].Trim();
							break;
						case "CapHeight":
							pdfFontDefinition.capHeight = int.Parse(array[1]);
							break;
						case "Ascender":
							pdfFontDefinition.ascender = int.Parse(array[1]);
							break;
						case "Descender":
							pdfFontDefinition.descender = int.Parse(array[1]);
							break;
						case "StdHW":
							pdfFontDefinition.StdHW = int.Parse(array[1]);
							break;
						case "StdVW":
							pdfFontDefinition.StdVW = int.Parse(array[1]);
							break;
						case "StartCharMetrics":
							flag = true;
							break;
						case "EndCharMetrics":
							flag = false;
							break;
						case "C":
							if (flag)
							{
								pdfCharacterMetric = getCharacterMetric(text);
								pdfFontDefinition.fontMetrics[pdfCharacterMetric.charIndex] = pdfCharacterMetric.charWidth;
							}
							break;
					}
				}
				return pdfFontDefinition;
			}
			catch (Exception)
			{
				throw new pdfBadFontFileException();
			}
		}

		private pdfCharacterMetric getCharacterMetric(string characterMetric)
		{
			string[] array = characterMetric.Split(csSemicolonToken);
			pdfCharacterMetric pdfCharacterMetric = null;
			int charWidth = 0;
			string text = null;
			string[] array2 = array;
			string[] array3;
			foreach (string text2 in array2)
			{
				array3 = text2.Trim().Split(csDelimiterToken);
				switch (array3[0])
				{
					case "C":
						int.Parse(array3[1]);
						break;
					case "WX":
						charWidth = int.Parse(array3[1]);
						break;
					case "N":
						text = array3[1];
						break;
				}
			}
			array = null;
			array3 = null;
			return new pdfCharacterMetric(text, GlyphConverter.UnicodeFromGlyph(text), charWidth);
		}

		public override void Dispose()
		{
			if (_afmStream != null)
			{
				_afmStream.Close();
				_afmStream = null;
			}
		}
	}
}
