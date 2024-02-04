using System;
using System.Collections;
using System.IO;
using sharpPDF.Exceptions;
using sharpPDF.Fonts.TTF.IO;

namespace sharpPDF.Fonts.TTF
{
	internal class ttfFontReader : FontReader
	{
		private AdvancedFileStream _fontStream = null;

		private OffsetTable _offsetTable = default(OffsetTable);

		private Hashtable _directoryTables = new Hashtable();

		private NamesTable _namesTable = default(NamesTable);

		private FontHeaderTable _headTable = default(FontHeaderTable);

		private HorizontalHeaderTable _hheaTable = default(HorizontalHeaderTable);

		private OSTable _osTable = default(OSTable);

		private PostScriptTable _postTable = default(PostScriptTable);

		private int[] _GlyphWidths;

		private ArrayList _CMAPTable = new ArrayList();

		private bool _disposed = false;

		public ttfFontReader(string fontReference)
			: base(fontReference)
		{
			if (!_fontReference.ToLower().EndsWith(".ttf"))
			{
				throw new pdfBadFontFileException();
			}
			try
			{
				_fontStream = new AdvancedFileStream(_fontReference, FileMode.Open);
			}
			catch (Exception ex)
			{
				throw new pdfGenericIOException(ex.Message, ex);
			}
		}

		public override void Dispose()
		{
			if (!_disposed)
			{
				_directoryTables = null;
				_CMAPTable = null;
				if (_fontStream != null)
				{
					_fontStream.Close();
					_fontStream = null;
				}
				_disposed = true;
			}
		}

		private void readFontFile()
		{
			try
			{
				readOffsetTable();
				_fontStream.Seek(12L, SeekOrigin.Begin);
				for (int i = 0; i < _offsetTable.NumberOfTables; i++)
				{
					DirectoryTable directoryTable = readDirectoryTable();
					_directoryTables.Add(directoryTable.Tag.ToLower(), directoryTable);
				}
				readNamesTable();
				readFontHeaderTable();
				readHorizontalHeaderTable();
				readOsTable();
				readPostScriptTable();
				readGlyphWidths();
				readCMAP();
			}
			catch (Exception)
			{
				throw new pdfBadFontFileException();
			}
		}

		public override pdfFontDefinition getFontDefinition()
		{
			pdfFontDefinition pdfFontDefinition = new pdfFontDefinition();
			try
			{
				readFontFile();
				pdfFontDefinition.fontName = "BBCDEE+" + _namesTable.FontName;
				pdfFontDefinition.fullFontName = _namesTable.FullName;
				pdfFontDefinition.familyName = _namesTable.FamilyName;
				pdfFontDefinition.ascender = _osTable.sTypoAscender * 1000 / _headTable.unitsPerEm;
				pdfFontDefinition.descender = _osTable.sTypoDescender * 1000 / _headTable.unitsPerEm;
				pdfFontDefinition.capHeight = _osTable.sCapHeight * 1000 / _headTable.unitsPerEm;
				pdfFontDefinition.fontBBox = new int[4]
				{
					_headTable.xMin * 1000 / _headTable.unitsPerEm,
					_headTable.yMin * 1000 / _headTable.unitsPerEm,
					_headTable.xMax * 1000 / _headTable.unitsPerEm,
					_headTable.yMax * 1000 / _headTable.unitsPerEm
				};
				pdfFontDefinition.fontHeight = Convert.ToInt32(Math.Round(((double)pdfFontDefinition.fontBBox[3] - (double)pdfFontDefinition.fontBBox[1]) / 1000.0));
				if (pdfFontDefinition.fontHeight == 0)
				{
					pdfFontDefinition.fontHeight = 1;
				}
				pdfFontDefinition.italicAngle = (int)_postTable.ItalicAngle * 1000 / _headTable.unitsPerEm;
				pdfFontDefinition.underlinePosition = _postTable.UnderlinePosition;
				pdfFontDefinition.underlineThickness = _postTable.UnderlineThickness;
				pdfFontDefinition.isFixedPitch = _postTable.IsFixedPitch;
				CMAPTable cMAPTable = null;
				cMAPTable = getCMAP(3, 1);
				if (cMAPTable == null)
				{
					cMAPTable = getCMAP(1, 0);
				}
				if (cMAPTable != null)
				{
					for (int i = 0; i < 65535; i++)
					{
						CharacterMetric characterMetric = default(CharacterMetric);
						if (cMAPTable.Mapping.ContainsKey(i))
						{
							characterMetric.characterMapping = ((int[])cMAPTable.Mapping[i])[0];
							characterMetric.characterWidth = ((int[])cMAPTable.Mapping[i])[1];
						}
						else
						{
							characterMetric.characterMapping = 0;
							characterMetric.characterWidth = 0;
						}
						pdfFontDefinition.fontMetrics[i] = characterMetric;
					}
					cMAPTable = null;
					return pdfFontDefinition;
				}
				throw new pdfCMAPNotSupportedException();
			}
			catch (pdfCMAPNotSupportedException)
			{
				throw new pdfCMAPNotSupportedException();
			}
			catch (Exception)
			{
				throw new pdfBadFontFileException();
			}
			finally
			{
				if (_fontStream != null)
				{
					_fontStream.Close();
				}
			}
		}

		public byte[] getFontStream()
		{
			_fontStream.SetPosition(0L);
			byte[] array = new byte[_fontStream.Length];
			_fontStream.Read(array, 0, array.Length);
			return array;
		}

		private void readOffsetTable()
		{
			_fontStream.SetPosition(0L);
			_fontStream.SkipBytes(4L);
			_offsetTable.Version = 1f;
			_offsetTable.NumberOfTables = _fontStream.readUShort_BE();
			_offsetTable.SearchRange = _fontStream.readUShort_BE();
			_offsetTable.EntrySelector = _fontStream.readUShort_BE();
			_offsetTable.RangeShift = _fontStream.readUShort_BE();
		}

		private void readNamesTable()
		{
			if (_directoryTables.ContainsKey("name"))
			{
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["name"];
				fontStream.SetPosition(directoryTable.Offset);
				_fontStream.SkipBytes(2L);
				int num = _fontStream.readUShort_BE();
				int num2 = _fontStream.readUShort_BE();
				int num3 = 0;
				bool flag = false;
				while (num3 < num && !flag)
				{
					int num4 = _fontStream.readUShort_BE();
					int num5 = _fontStream.readUShort_BE();
					_fontStream.readUShort_BE();
					int num6 = _fontStream.readUShort_BE();
					int length = _fontStream.readUShort_BE();
					int num7 = _fontStream.readUShort_BE();
					long position = _fontStream.Position;
					AdvancedFileStream fontStream2 = _fontStream;
					directoryTable = (DirectoryTable)_directoryTables["name"];
					fontStream2.SetPosition(directoryTable.Offset + num2 + num7);
					switch (num6)
					{
					case 1:
						if (num4 == 0 || num4 == 3 || (num4 == 2 && num5 == 1))
						{
							_namesTable.FamilyName = _fontStream.readUnicodeString(length);
						}
						else
						{
							_namesTable.FamilyName = _fontStream.readString(length);
						}
						break;
					case 4:
						if (num4 == 0 || num4 == 3 || (num4 == 2 && num5 == 1))
						{
							_namesTable.FullName = _fontStream.readUnicodeString(length);
						}
						else
						{
							_namesTable.FullName = _fontStream.readString(length);
						}
						break;
					case 6:
						if (num4 == 0 || num4 == 3 || (num4 == 2 && num5 == 1))
						{
							_namesTable.FontName = _fontStream.readUnicodeString(length).Replace(" ", "");
						}
						else
						{
							_namesTable.FontName = _fontStream.readString(length).Replace(" ", "");
						}
						break;
					}
					if (_namesTable.FontName != null && _namesTable.FamilyName != null && _namesTable.FullName != null)
					{
						flag = true;
						continue;
					}
					_fontStream.SetPosition(position);
					num3++;
				}
				if (!flag)
				{
					throw new pdfBadFontFileException();
				}
				return;
			}
			throw new pdfBadFontFileException();
		}

		private DirectoryTable readDirectoryTable()
		{
			DirectoryTable result = default(DirectoryTable);
			result.Tag = _fontStream.readString(4);
			result.Checksum = _fontStream.readULong_BE();
			result.Offset = _fontStream.readULong_BE();
			result.Length = _fontStream.readULong_BE();
			return result;
		}

		private void readFontHeaderTable()
		{
			if (_directoryTables.ContainsKey("head"))
			{
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["head"];
				fontStream.SetPosition(directoryTable.Offset);
				_fontStream.SkipBytes(16L);
				_headTable.flags = _fontStream.readUShort_BE();
				_headTable.unitsPerEm = _fontStream.readUShort_BE();
				_fontStream.SkipBytes(16L);
				_headTable.xMin = _fontStream.readShort_BE();
				_headTable.yMin = _fontStream.readShort_BE();
				_headTable.xMax = _fontStream.readShort_BE();
				_headTable.yMax = _fontStream.readShort_BE();
				_headTable.macStyle = _fontStream.readUShort_BE();
				return;
			}
			throw new pdfBadFontFileException();
		}

		private void readHorizontalHeaderTable()
		{
			if (_directoryTables.ContainsKey("hhea"))
			{
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["hhea"];
				fontStream.SetPosition(directoryTable.Offset);
				_fontStream.SkipBytes(4L);
				_hheaTable.Ascender = _fontStream.readShort_BE();
				_hheaTable.Descender = _fontStream.readShort_BE();
				_hheaTable.LineGap = _fontStream.readShort_BE();
				_hheaTable.advanceWidthMax = _fontStream.readUShort_BE();
				_hheaTable.minLeftSideBearing = _fontStream.readShort_BE();
				_hheaTable.minRightSideBearing = _fontStream.readShort_BE();
				_hheaTable.xMaxExtent = _fontStream.readShort_BE();
				_hheaTable.caretSlopeRise = _fontStream.readShort_BE();
				_hheaTable.caretSlopeRun = _fontStream.readShort_BE();
				_fontStream.SkipBytes(12L);
				_hheaTable.numberOfHMetrics = _fontStream.readUShort_BE();
				return;
			}
			throw new pdfBadFontFileException();
		}

		private void readOsTable()
		{
			if (_directoryTables.ContainsKey("os/2"))
			{
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["os/2"];
				fontStream.SetPosition(directoryTable.Offset);
				int num = _fontStream.readShort_BE();
				_osTable.xAvgCharWidth = _fontStream.readShort_BE();
				_osTable.usWeightClass = _fontStream.readUShort_BE();
				_osTable.usWidthClass = _fontStream.readUShort_BE();
				_osTable.fsType = _fontStream.readShort_BE();
				_osTable.ySubscriptXSize = _fontStream.readShort_BE();
				_osTable.ySubscriptYSize = _fontStream.readShort_BE();
				_osTable.ySubscriptXOffset = _fontStream.readShort_BE();
				_osTable.ySubscriptYOffset = _fontStream.readShort_BE();
				_osTable.ySuperscriptXSize = _fontStream.readShort_BE();
				_osTable.ySuperscriptYSize = _fontStream.readShort_BE();
				_osTable.ySuperscriptXOffset = _fontStream.readShort_BE();
				_osTable.ySuperscriptYOffset = _fontStream.readShort_BE();
				_osTable.yStrikeoutSize = _fontStream.readShort_BE();
				_osTable.yStrikeoutPosition = _fontStream.readShort_BE();
				_osTable.sFamilyClass = _fontStream.readShort_BE();
				_osTable.panose = _fontStream.readByteArray(10);
				_fontStream.SkipBytes(16L);
				_osTable.achVendID = _fontStream.readString(4);
				_osTable.fsSelection = _fontStream.readUShort_BE();
				_osTable.usFirstCharIndex = _fontStream.readUShort_BE();
				_osTable.usLastCharIndex = _fontStream.readUShort_BE();
				_osTable.sTypoAscender = _fontStream.readShort_BE();
				_osTable.sTypoDescender = _fontStream.readShort_BE();
				_osTable.sTypoLineGap = _fontStream.readShort_BE();
				_osTable.usWinAscent = _fontStream.readUShort_BE();
				_osTable.usWinDescent = _fontStream.readUShort_BE();
				_osTable.ulCodePageRange1 = 0;
				_osTable.ulCodePageRange2 = 0;
				if (num > 0)
				{
					_osTable.ulCodePageRange1 = _fontStream.readInt_BE();
					_osTable.ulCodePageRange2 = _fontStream.readInt_BE();
				}
				if (num > 1)
				{
					_fontStream.SkipBytes(2L);
					_osTable.sCapHeight = _fontStream.readShort_BE();
				}
				else
				{
					_osTable.sCapHeight = (int)(0.7 * (double)_headTable.unitsPerEm);
				}
				return;
			}
			throw new pdfBadFontFileException();
		}

		private void readPostScriptTable()
		{
			if (_directoryTables.ContainsKey("post"))
			{
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["post"];
				fontStream.SetPosition(directoryTable.Offset);
				_fontStream.SkipBytes(4L);
				short num = _fontStream.readShort_BE();
				int num2 = _fontStream.readUShort_BE();
				_postTable.ItalicAngle = (double)num + (double)num2 / 16384.0;
				_postTable.UnderlinePosition = _fontStream.readInt_BE();
				_postTable.UnderlineThickness = _fontStream.readInt_BE();
				_postTable.IsFixedPitch = _fontStream.readInt_BE() != 0;
				return;
			}
			throw new pdfBadFontFileException();
		}

		private void readGlyphWidths()
		{
			if (_directoryTables.ContainsKey("hmtx"))
			{
				_GlyphWidths = new int[_hheaTable.numberOfHMetrics];
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["hmtx"];
				fontStream.SetPosition(directoryTable.Offset);
				for (int i = 0; i < _hheaTable.numberOfHMetrics; i++)
				{
					_GlyphWidths[i] = _fontStream.readUShort_BE() * 1000 / _headTable.unitsPerEm;
					_fontStream.SkipBytes(2L);
				}
				return;
			}
			throw new pdfBadFontFileException();
		}

		private void readCMAP()
		{
			if (_directoryTables.ContainsKey("cmap"))
			{
				AdvancedFileStream fontStream = _fontStream;
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["cmap"];
				fontStream.SetPosition(directoryTable.Offset);
				_fontStream.SkipBytes(2L);
				int num = _fontStream.readUShort_BE();
				for (int i = 0; i < num; i++)
				{
					int platformID = _fontStream.readUShort_BE();
					int encodingID = _fontStream.readUShort_BE();
					int offset = _fontStream.readULong_BE();
					_CMAPTable.Add(new CMAPTable(platformID, encodingID, offset));
				}
				{
					foreach (CMAPTable item in _CMAPTable)
					{
						AdvancedFileStream fontStream2 = _fontStream;
						directoryTable = (DirectoryTable)_directoryTables["cmap"];
						fontStream2.SetPosition(directoryTable.Offset + item.Offset);
						int num2 = _fontStream.readUShort_BE();
						switch (item.PlatformID)
						{
						case 1:
							switch (num2)
							{
							case 0:
								item.Mapping = readFormat0();
								break;
							case 4:
								item.Mapping = readFormat4();
								break;
							case 6:
								item.Mapping = readFormat6();
								break;
							}
							break;
						case 3:
							if (num2 == 4)
							{
								item.Mapping = readFormat4();
							}
							break;
						}
					}
					return;
				}
			}
			throw new pdfBadFontFileException();
		}

		private Hashtable readFormat0()
		{
			Hashtable hashtable = new Hashtable();
			_fontStream.SkipBytes(2L);
			_fontStream.SkipBytes(2L);
			for (int i = 0; i < 256; i++)
			{
				int[] array = new int[2];
				array[0] = _fontStream.readByte();
				array[1] = _GlyphWidths[array[0]];
				hashtable.Add(i, array);
				array = null;
			}
			return hashtable;
		}

		private Hashtable readFormat4()
		{
			Hashtable hashtable = new Hashtable();
			int num = _fontStream.readUShort_BE();
			_fontStream.SkipBytes(2L);
			int num2 = _fontStream.readUShort_BE() / 2;
			_fontStream.SkipBytes(6L);
			int[] array = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = _fontStream.readUShort_BE();
			}
			_fontStream.SkipBytes(2L);
			int[] array2 = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array2[i] = _fontStream.readUShort_BE();
			}
			int[] array3 = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array3[i] = _fontStream.readUShort_BE();
			}
			int[] array4 = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array4[i] = _fontStream.readUShort_BE();
			}
			int[] array5 = new int[num / 2 - 8 - num2 * 4];
			for (int i = 0; i < array5.Length; i++)
			{
				array5[i] = _fontStream.readUShort_BE();
			}
			for (int i = 0; i < num2; i++)
			{
				for (int j = array2[i]; j <= array[i] && j != 65535; j++)
				{
					int num3;
					if (array4[i] == 0)
					{
						num3 = (j + array3[i]) & 0xFFFF;
					}
					else
					{
						int num4 = i + array4[i] / 2 - num2 + j - array2[i];
						num3 = (array5[num4] + array3[i]) & 0xFFFF;
					}
					int[] array6 = new int[2];
					array6[0] = num3;
					array6[1] = _GlyphWidths[array6[0]];
					hashtable.Add(j, array6);
				}
			}
			return hashtable;
		}

		private Hashtable readFormat6()
		{
			Hashtable hashtable = new Hashtable();
			_fontStream.SkipBytes(2L);
			_fontStream.SkipBytes(2L);
			int num = _fontStream.readUShort_BE();
			int num2 = _fontStream.readUShort_BE();
			for (int i = 0; i < num2; i++)
			{
				int[] array = new int[2];
				array[0] = _fontStream.readByte();
				array[1] = _GlyphWidths[array[0]];
				hashtable.Add(i + num, array);
				array = null;
			}
			return hashtable;
		}

		private CMAPTable getCMAP(int platformID, int encodingID)
		{
			CMAPTable cMAPTable = null;
			for (int i = 0; i < _CMAPTable.Count; i++)
			{
				if (cMAPTable != null)
				{
					break;
				}
				if (((CMAPTable)_CMAPTable[i]).PlatformID == platformID && ((CMAPTable)_CMAPTable[i]).EncodingID == encodingID)
				{
					cMAPTable = (CMAPTable)_CMAPTable[i];
				}
			}
			return cMAPTable;
		}
	}
}
