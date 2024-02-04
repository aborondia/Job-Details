using System;
using System.Collections;
using System.IO;
using sharpPDF.Exceptions;
using sharpPDF.Fonts.TTF.IO;

namespace sharpPDF.Fonts.TTF
{
	internal class ttfFontSubset : IDisposable
	{
		internal static int[] entrySelectors = new int[21]
		{
			0, 0, 1, 1, 2, 2, 2, 2, 3, 3,
			3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
			4
		};

		internal static int FORMAT_OFFSET = 51;

		private string _baseFileName;

		private AdvancedFileStream _fontStream;

		private Hashtable _directoryTables;

		private bool _isShortLocaTable;

		private int[] _locaTable;

		private Hashtable _usedGlyphs;

		private ArrayList _glyphKeys;

		private int[] _newLocaTable;

		private byte[] _newLocaTableOut;

		private byte[] _newGlyfTable;

		private int _realGlyfSize;

		private int _realLocaSize;

		private int _oldGlyfOffset;

		private int _directoryOffset = 0;

		private bool _disposed = false;

		private AdvancedMemoryStream _subsetStream;

		public ttfFontSubset(string baseFileName, Hashtable usedCharacters)
		{
			_baseFileName = baseFileName;
			_usedGlyphs = usedCharacters;
			_glyphKeys = new ArrayList(_usedGlyphs.Keys);
			_subsetStream = new AdvancedMemoryStream();
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				if (_subsetStream != null)
				{
					_subsetStream.Close();
					_subsetStream = null;
				}
				_glyphKeys = null;
				_disposed = true;
			}
		}

		public byte[] getFontFileStream()
		{
			byte[] array = null;
			try
			{
				_fontStream = new AdvancedFileStream(_baseFileName, FileMode.Open);
				_subsetStream = new AdvancedMemoryStream();
				CreateDirectoryTable();
				ReadLocaTable();
				FlatGlyphs();
				CreateGlyphTables();
				LocaOldTobytesLocaNew();
				CreateFontSubset();
				return _subsetStream.ToArray();
			}
			catch (IOException ex)
			{
				throw new pdfGenericIOException(ex.Message, ex);
			}
			finally
			{
				if (_fontStream != null)
				{
					_fontStream.Close();
					_fontStream = null;
				}
				if (_subsetStream != null)
				{
					_subsetStream.Close();
					_subsetStream = null;
				}
			}
		}

		private void CreateDirectoryTable()
		{
			_directoryTables = new Hashtable();
			_fontStream.SetPosition(_directoryOffset);
			_fontStream.SkipBytes(4L);
			int num = _fontStream.readUShort_BE();
			_fontStream.SkipBytes(6L);
			for (int i = 0; i < num; i++)
			{
				DirectoryTable directoryTable = ReadDirectoryTable();
				_directoryTables.Add(directoryTable.Tag.ToLower(), directoryTable);
			}
		}

		private DirectoryTable ReadDirectoryTable()
		{
			DirectoryTable result = default(DirectoryTable);
			result.Tag = _fontStream.readString(4);
			result.Checksum = _fontStream.readULong_BE();
			result.Offset = _fontStream.readULong_BE();
			result.Length = _fontStream.readULong_BE();
			return result;
		}

		private void ReadLocaTable()
		{
			if (_directoryTables.Contains("head") && _directoryTables.Contains("loca"))
			{
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["head"];
				_fontStream.SetPosition(directoryTable.Offset + FORMAT_OFFSET);
				_isShortLocaTable = _fontStream.readUShort_BE() == 0;
				directoryTable = (DirectoryTable)_directoryTables["loca"];
				_fontStream.SetPosition(directoryTable.Offset);
				if (_isShortLocaTable)
				{
					int num = directoryTable.Length / 2;
					_locaTable = new int[num];
					for (int i = 0; i < num; i++)
					{
						_locaTable[i] = _fontStream.readUShort_BE() * 2;
					}
				}
				else
				{
					int num = directoryTable.Length / 4;
					_locaTable = new int[num];
					for (int i = 0; i < num; i++)
					{
						_locaTable[i] = _fontStream.readULong_BE();
					}
				}
				return;
			}
			throw new pdfBadFontFileException();
		}

		protected void LocaOldTobytesLocaNew()
		{
			int num = 0;
			if (_isShortLocaTable)
			{
				_realLocaSize = _newLocaTable.Length * 2;
			}
			else
			{
				_realLocaSize = _newLocaTable.Length * 4;
			}
			_newLocaTableOut = new byte[(_realLocaSize + 3) & -4];
			for (int i = 0; i < _newLocaTable.Length; i++)
			{
				if (_isShortLocaTable)
				{
					int num2 = _newLocaTable[i] / 2;
					_newLocaTableOut[num] = (byte)(num2 >> 8);
					num++;
					_newLocaTableOut[num] = (byte)((uint)num2 & 0xFFu);
					num++;
				}
				else
				{
					int num2 = _newLocaTable[i];
					_newLocaTableOut[num] = (byte)(num2 >> 24);
					num++;
					_newLocaTableOut[num] = (byte)((uint)(num2 >> 16) & 0xFFu);
					num++;
					_newLocaTableOut[num] = (byte)((uint)(num2 >> 8) & 0xFFu);
					num++;
					_newLocaTableOut[num] = (byte)((uint)num2 & 0xFFu);
					num++;
				}
			}
		}

		protected void CreateFontSubset()
		{
			int num = 0;
			string[] array = new string[10] { "cmap", "cvt ", "fpgm", "glyf", "head", "hhea", "hmtx", "loca", "maxp", "prep" };
			int num2 = 2;
			int num3 = 0;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!text.Equals("glyf") && !text.Equals("loca") && _directoryTables.Contains(text))
				{
					DirectoryTable directoryTable = (DirectoryTable)_directoryTables[text];
					num += (directoryTable.Length + 3) & -4;
					num2++;
				}
			}
			num += _newLocaTableOut.Length;
			num += _newGlyfTable.Length;
			int num4 = 16 * num2 + 12;
			num += num4;
			_subsetStream.writeULong_BE(65536);
			_subsetStream.writeUShort_BE(num2);
			int num5 = entrySelectors[num2];
			_subsetStream.writeUShort_BE((1 << num5) * 16);
			_subsetStream.writeUShort_BE(num5);
			_subsetStream.writeUShort_BE((num2 - (1 << num5)) * 16);
			array2 = array;
			foreach (string text2 in array2)
			{
				if (_directoryTables.Contains(text2))
				{
					DirectoryTable directoryTable = (DirectoryTable)_directoryTables[text2];
					_subsetStream.writeString(text2);
					if (text2.Equals("glyf"))
					{
						_subsetStream.writeULong_BE(CalculateChecksum(_newGlyfTable));
						num3 = _realGlyfSize;
					}
					else if (text2.Equals("loca"))
					{
						_subsetStream.writeULong_BE(CalculateChecksum(_newLocaTableOut));
						num3 = _realLocaSize;
					}
					else
					{
						_subsetStream.writeULong_BE(directoryTable.Checksum);
						num3 = directoryTable.Length;
					}
					_subsetStream.writeULong_BE(num4);
					_subsetStream.writeULong_BE(num3);
					num4 += (num3 + 3) & -4;
				}
			}
			array2 = array;
			foreach (string text3 in array2)
			{
				if (_directoryTables.Contains(text3))
				{
					DirectoryTable directoryTable = (DirectoryTable)_directoryTables[text3];
					if (text3.Equals("glyf"))
					{
						_subsetStream.writeByteArray(_newGlyfTable);
						_newGlyfTable = null;
						continue;
					}
					if (text3.Equals("loca"))
					{
						_subsetStream.writeByteArray(_newLocaTableOut);
						_newLocaTableOut = null;
						continue;
					}
					_fontStream.SetPosition(directoryTable.Offset);
					byte[] array3 = new byte[(directoryTable.Length + 3) & -4];
					array3.Initialize();
					_fontStream.readByteArray(directoryTable.Length).CopyTo(array3, 0);
					_subsetStream.writeByteArray(array3);
				}
			}
		}

		protected void FlatGlyphs()
		{
			if (_directoryTables.Contains("glyf"))
			{
				DirectoryTable directoryTable = (DirectoryTable)_directoryTables["glyf"];
				_oldGlyfOffset = directoryTable.Offset;
				if (!_usedGlyphs.ContainsKey(0))
				{
					_usedGlyphs.Add(0, null);
					_glyphKeys.Add(0);
				}
				for (int i = 0; i < _glyphKeys.Count; i++)
				{
					CheckGlyphComp((int)_glyphKeys[i]);
				}
				return;
			}
			throw new pdfBadFontFileException();
		}

		protected void CheckGlyphComp(int glyph)
		{
			int num = _locaTable[glyph];
			if (num == _locaTable[glyph + 1])
			{
				return;
			}
			_fontStream.SetPosition(_oldGlyfOffset + num);
			int num2 = _fontStream.readShort_BE();
			if (num2 >= 0)
			{
				return;
			}
			_fontStream.SkipBytes(8L);
			bool flag = false;
			while (!flag)
			{
				int num3 = _fontStream.readUShort_BE();
				int num4 = _fontStream.readUShort_BE();
				if (!_usedGlyphs.ContainsKey(num4))
				{
					_usedGlyphs.Add(num4, null);
					_glyphKeys.Add(num4);
				}
				if ((num3 & 0x20) == 0)
				{
					flag = true;
				}
				int num5 = 0;
				num5 = (((num3 & 1) == 0) ? 2 : 4);
				if (((uint)num3 & 8u) != 0)
				{
					num5 += 2;
				}
				else if (((uint)num3 & 0x40u) != 0)
				{
					num5 += 4;
				}
				if (((uint)num3 & 0x80u) != 0)
				{
					num5 += 8;
				}
				_fontStream.SkipBytes(num5);
			}
		}

		protected void CreateGlyphTables()
		{
			_newLocaTable = new int[_locaTable.Length];
			int[] array = new int[_glyphKeys.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (int)_glyphKeys[i];
			}
			Array.Sort((Array)array);
			_realGlyfSize = 0;
			foreach (object glyphKey in _glyphKeys)
			{
				int num = (int)glyphKey;
				_realGlyfSize += _locaTable[num + 1] - _locaTable[num];
			}
			_newGlyfTable = new byte[(_realGlyfSize + 3) & -4];
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < _newLocaTable.Length; j++)
			{
				_newLocaTable[j] = num2;
				if (num3 < array.Length && array[num3] == j)
				{
					num3++;
					_newLocaTable[j] = num2;
					int num4 = _locaTable[j];
					int num5 = _locaTable[j + 1] - num4;
					if (num5 > 0)
					{
						_fontStream.SetPosition(_oldGlyfOffset + num4);
						_fontStream.readByteArray(num5).CopyTo(_newGlyfTable, num2);
						num2 += num5;
					}
				}
			}
		}

		protected int CalculateChecksum(byte[] byteArray)
		{
			int num = byteArray.Length / 4;
			int[] array = new int[4];
			int[] array2 = array;
			for (int i = 0; i < num; i += 4)
			{
				(array = array2)[3] = array[3] + (byteArray[i] & 0xFF);
				(array = array2)[2] = array[2] + (byteArray[i + 1] & 0xFF);
				(array = array2)[1] = array[1] + (byteArray[i + 2] & 0xFF);
				(array = array2)[0] = array[0] + (byteArray[i + 3] & 0xFF);
			}
			return array2[0] + (array2[1] << 8) + (array2[2] << 16) + (array2[3] << 24);
		}
	}
}
