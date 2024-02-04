using System;
using System.Text;
using sharpPDF.Collections;
using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;

namespace sharpPDF
{
	public abstract class textAdapter
	{
		public static string HEXFormatter(string strText)
		{
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = strText.ToCharArray();
			foreach (char value in array)
			{
				stringBuilder.Append(Convert.ToByte(value).ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		public static string checkText(string strText)
		{
			string text = strText;
			text = text.Replace("\\", "\\\\");
			text = text.Replace("(", "\\(");
			return text.Replace(")", "\\)");
		}

		private static paragraphLine createNewLine(string text, predefinedAlignment parAlign, int parWidth, int lineLength, int lineHeight, pdfAbstractFont fontType)
		{
			paragraphLine result = parAlign switch
			{
				predefinedAlignment.csRight => new paragraphLine(text, lineHeight, parWidth - lineLength, fontType), 
				predefinedAlignment.csCenter => new paragraphLine(text, lineHeight, Convert.ToInt32(Math.Round(((double)parWidth - (double)lineLength) / 2.0)), fontType), 
				_ => new paragraphLine(text, lineHeight, 0, fontType), 
			};
			if (fontType is pdfTrueTypeFont)
			{
				((pdfTrueTypeFont)fontType).addCharacters(text);
			}
			return result;
		}

		public static paragraphLineList formatParagraph(ref string strText, int fontSize, pdfAbstractFont fontType, int parWidth, int maxLines, int lineHeight)
		{
			return formatParagraph(ref strText, fontSize, fontType, parWidth, maxLines, lineHeight, predefinedAlignment.csLeft);
		}

		public static paragraphLineList formatParagraph(ref string strText, int fontSize, pdfAbstractFont fontType, int parWidth, int maxLines, int lineHeight, predefinedAlignment parAlign)
		{
			string[] array = strText.Replace("\r", "").Split('\n');
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			paragraphLineList paragraphLineList = new paragraphLineList();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (!flag && num2 < array.Length)
			{
				string[] array2 = array[num2].Split(" ".ToCharArray());
				num3 = 0;
				while (!flag && num3 < array2.Length)
				{
					string text = fontType.cleanText(array2[num3]);
					if (text.Trim() != "")
					{
						if (fontType.getWordWidth(text + " ", fontSize) + num > parWidth)
						{
							if (num == 0)
							{
								paragraphLineList.Add(createNewLine(fontType.cropWord(text, parWidth, fontSize), parAlign, parWidth, parWidth, lineHeight, fontType));
								strText.Remove(0, array2[num3].Length).Trim();
								num3++;
							}
							else
							{
								paragraphLineList.Add(createNewLine(stringBuilder.ToString().Trim(), parAlign, parWidth, num, lineHeight, fontType));
								stringBuilder.Remove(0, stringBuilder.Length);
								num = 0;
							}
							if (paragraphLineList.Count == maxLines && maxLines > 0)
							{
								flag = true;
							}
						}
						else
						{
							stringBuilder.Append(text + " ");
							num += fontType.getWordWidth(text + " ", fontSize);
							strText = strText.Remove(0, array2[num3].Length).Trim();
							num3++;
						}
					}
					else
					{
						num3++;
					}
				}
				num2++;
				if (!flag)
				{
					if (num > 0)
					{
						paragraphLineList.Add(createNewLine(stringBuilder.ToString().Trim(), parAlign, parWidth, num, lineHeight, fontType));
						stringBuilder.Remove(0, stringBuilder.Length);
						num = 0;
					}
					else
					{
						paragraphLineList.Add(new paragraphLine("", lineHeight, 0, fontType));
					}
					if (paragraphLineList.Count == maxLines && maxLines > 0)
					{
						flag = true;
					}
				}
				array2 = null;
			}
			return paragraphLineList;
		}
	}
}
