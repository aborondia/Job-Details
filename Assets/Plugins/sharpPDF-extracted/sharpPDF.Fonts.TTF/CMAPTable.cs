using System.Collections;

namespace sharpPDF.Fonts.TTF
{
	internal class CMAPTable
	{
		public int PlatformID;

		public int EncodingID;

		public int Offset;

		public Hashtable Mapping;

		public CMAPTable(int platformID, int encodingID, int offset)
		{
			PlatformID = platformID;
			EncodingID = encodingID;
			Offset = offset;
			Mapping = new Hashtable();
		}
	}
}
