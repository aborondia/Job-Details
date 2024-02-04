using System;
using System.IO;
using System.Text;
using sharpPDF.Exceptions;

namespace sharpPDF.Fonts.TTF.IO
{
	internal class AdvancedFileStream : FileStream
	{
		public AdvancedFileStream(string fileName, FileMode fileMode)
			: base(fileName, fileMode)
		{
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

		public int readULong_BE()
		{
			byte[] array = new byte[4];
			int num = 0;
			Read(array, 0, array.Length);
			num = (array[0] << 24) + (array[1] << 16) + (array[2] << 8) + array[3];
			array = null;
			return num;
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

		public int readUShort_BE()
		{
			byte[] array = new byte[2];
			int num = 0;
			Read(array, 0, array.Length);
			num = (array[0] << 8) + array[1];
			array = null;
			return num;
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

		public int readInt_BE()
		{
			byte[] array = new byte[2];
			int num = 0;
			Read(array, 0, array.Length);
			num = (array[0] << 8) + array[1];
			array = null;
			return num;
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

		public byte[] readByteArray_BE(int length)
		{
			byte[] array = new byte[length];
			Read(array, 0, array.Length);
			return Encoding.Convert(Encoding.BigEndianUnicode, Encoding.Unicode, array, 0, array.Length);
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
