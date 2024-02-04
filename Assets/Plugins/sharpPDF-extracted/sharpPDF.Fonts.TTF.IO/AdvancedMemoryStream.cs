using System;
using System.Collections;
using System.IO;
using System.Text;
using sharpPDF.Exceptions;

namespace sharpPDF.Fonts.TTF.IO
{
	internal class AdvancedMemoryStream : MemoryStream
	{
		private Hashtable winansi = new Hashtable();

		public AdvancedMemoryStream()
		{
			winansi[8364] = 128;
			winansi[8218] = 130;
			winansi[402] = 131;
			winansi[8222] = 132;
			winansi[8230] = 133;
			winansi[8224] = 134;
			winansi[8225] = 135;
			winansi[710] = 136;
			winansi[8240] = 137;
			winansi[352] = 138;
			winansi[8249] = 139;
			winansi[338] = 140;
			winansi[381] = 142;
			winansi[8216] = 145;
			winansi[8217] = 146;
			winansi[8220] = 147;
			winansi[8221] = 148;
			winansi[8226] = 149;
			winansi[8211] = 150;
			winansi[8212] = 151;
			winansi[732] = 152;
			winansi[8482] = 153;
			winansi[353] = 154;
			winansi[8250] = 155;
			winansi[339] = 156;
			winansi[382] = 158;
			winansi[376] = 159;
		}

		public string readString(int length)
		{
			try
			{
				byte[] array = new byte[length];
				StringBuilder stringBuilder = new StringBuilder();
				Read(array, 0, array.Length);
				for (int i = 0; i < length; i++)
				{
					stringBuilder.Append(Convert.ToChar(array[i]));
				}
				return stringBuilder.ToString();
			}
			catch (IOException ex)
			{
				throw new pdfGenericIOException(ex.Message, ex);
			}
		}

		public void writeString(string stringValue)
		{
			byte[] array = new byte[stringValue.Length];
			for (int i = 0; i < stringValue.Length; i++)
			{
				if (stringValue[i] < '\u0080' || (stringValue[i] >= '\u00a0' && stringValue[i] <= 'ÿ'))
				{
					array[i] = Convert.ToByte((int)stringValue[i]);
				}
				else
				{
					array[i] = Convert.ToByte((int)winansi[stringValue[i]]);
				}
			}
			try
			{
				Write(array, 0, array.Length);
			}
			catch (Exception ex)
			{
				throw new pdfGenericIOException(ex.Message, ex);
			}
		}

		public string readUnicodeString(int length)
		{
			byte[] array = new byte[length];
			StringBuilder stringBuilder = new StringBuilder();
			Read(array, 0, array.Length);
			for (int i = 0; i < length; i += 2)
			{
				stringBuilder.Append(Convert.ToChar((array[i] << 8) + array[i + 1]));
			}
			return stringBuilder.ToString();
		}

		public void writeUnicodeString(string stringValue)
		{
			Write(Encoding.Unicode.GetBytes(stringValue), 0, Encoding.Unicode.GetBytes(stringValue).Length);
		}

		public int readULong_BE()
		{
			byte[] array = new byte[4];
			int num = 0;
			Read(array, 0, array.Length);
			num = (array[0] << 24) + (array[1] << 16) + (array[2] << 8) + array[3];
			array = null;
			return num;
		}

		public void writeULong_BE(int intValue)
		{
			byte[] array = new byte[4]
			{
				(byte)(intValue >> 24),
				(byte)((uint)(intValue >> 16) & 0xFFu),
				(byte)((uint)(intValue >> 8) & 0xFFu),
				(byte)((uint)intValue & 0xFFu)
			};
			Write(array, 0, array.Length);
			array = null;
		}

		public short readShort_BE()
		{
			byte[] array = new byte[2];
			short num = 0;
			Read(array, 0, array.Length);
			num = (short)((array[0] << 8) + array[1]);
			array = null;
			return num;
		}

		public void writeShort_BE(short shortValue)
		{
			byte[] array = new byte[2]
			{
				(byte)(shortValue >> 8),
				(byte)((uint)shortValue & 0xFFu)
			};
			Write(array, 0, array.Length);
			array = null;
		}

		public int readUShort_BE()
		{
			byte[] array = new byte[2];
			int num = 0;
			Read(array, 0, array.Length);
			num = (array[0] << 8) + array[1];
			array = null;
			return num;
		}

		public void writeUShort_BE(int ushortValue)
		{
			byte[] array = new byte[2]
			{
				(byte)(ushortValue >> 8),
				(byte)((uint)ushortValue & 0xFFu)
			};
			Write(array, 0, array.Length);
			array = null;
		}

		public int readInt()
		{
			byte[] array = new byte[2];
			int num = 0;
			Read(array, 0, array.Length);
			num = array[1] + array[0];
			array = null;
			return num;
		}

		public void writeInt(int intValue)
		{
			byte[] array = new byte[2]
			{
				(byte)((uint)intValue & 0xFFu),
				(byte)(intValue >> 8)
			};
			Write(array, 0, array.Length);
			array = null;
		}

		public int readInt_BE()
		{
			byte[] array = new byte[2];
			int num = 0;
			Read(array, 0, array.Length);
			num = (array[0] << 8) + array[1];
			array = null;
			return num;
		}

		public void writeInt_BE(int intValue)
		{
			byte[] array = new byte[2]
			{
				(byte)(intValue >> 8),
				(byte)((uint)intValue & 0xFFu)
			};
			Write(array, 0, array.Length);
			array = null;
		}

		public byte readByte()
		{
			byte[] array = new byte[1];
			byte b = 0;
			Read(array, 0, array.Length);
			b = array[0];
			array = null;
			return b;
		}

		public byte[] readByteArray(int length)
		{
			byte[] array = new byte[length];
			Read(array, 0, array.Length);
			return array;
		}

		public void writeByteArray(byte[] byteArrayValue)
		{
			Write(byteArrayValue, 0, byteArrayValue.Length);
		}

		public byte[] readByteArray_BE(int length)
		{
			byte[] array = new byte[length];
			Read(array, 0, array.Length);
			return Encoding.Convert(Encoding.BigEndianUnicode, Encoding.Unicode, array, 0, array.Length);
		}

		public void writeByteArray_BE(byte[] byteArrayValue)
		{
			byte[] array = Encoding.Convert(Encoding.Unicode, Encoding.BigEndianUnicode, byteArrayValue, 0, byteArrayValue.Length);
			Write(array, 0, array.Length);
			array = null;
		}

		public void SetPosition(long offset)
		{
			base.Position = offset;
		}

		public void SkipBytes(long length)
		{
			base.Position += length;
		}
	}
}
