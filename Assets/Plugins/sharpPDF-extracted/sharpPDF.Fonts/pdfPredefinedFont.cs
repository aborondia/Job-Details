using System;
using System.Text;
using sharpPDF.Enumerators;

namespace sharpPDF.Fonts
{
	public sealed class pdfPredefinedFont : pdfAbstractFont
	{
		internal pdfPredefinedFont(pdfFontDefinition fontDefinition, int fontNumber, documentFontEncoding encodingType)
			: base(fontDefinition, fontNumber, encodingType)
		{
		}

		public override string getText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_objectID.ToString() + " 0 obj" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("<<" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Type /Font" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Subtype /Type1" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Name /F" + _fontNumber.ToString() + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/BaseFont /" + _fontDefinition.fontName + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("/Encoding /WinAnsiEncoding" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append(">>" + Convert.ToChar(13) + Convert.ToChar(10));
			stringBuilder.Append("endobj" + Convert.ToChar(13) + Convert.ToChar(10));
			return stringBuilder.ToString();
		}

		public override string encodeText(string strText)
		{
			char[] array = strText.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				if (Convert.ToInt32(array[i]) < 0 || Convert.ToInt32(array[i]) > 255)
				{
					stringBuilder.Append(GlyphConverter.pdfCodeFromUnicode(Convert.ToInt32(array[i])));
				}
				else
				{
					stringBuilder.Append(array[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public override string cleanText(string strText)
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = strText.ToCharArray();
			foreach (char value in array)
			{
				if (Convert.ToInt32(value) >= 0 && Convert.ToInt32(value) <= 255)
				{
					stringBuilder.Append(value);
				}
				else if (GlyphConverter.pdfCodeFromUnicode(Convert.ToInt32(value)) != "")
				{
					stringBuilder.Append(value);
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
				if ((Convert.ToInt32(c) >= 0 && Convert.ToInt32(c) <= 255) || GlyphConverter.pdfCodeFromUnicode(Convert.ToInt32(c)) != "")
				{
					num += (double)(int)_fontDefinition.fontMetrics[(uint)c];
				}
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
