using System;

namespace sharpPDF.Fonts
{
	public struct CharacterMetric : IComparable
	{
		public int characterMapping;

		public int characterWidth;

		public int CompareTo(object obj)
		{
			if (obj is CharacterMetric)
			{
				ref int reference = ref characterMapping;
				CharacterMetric characterMetric = (CharacterMetric)obj;
				return reference.CompareTo((object)characterMetric.characterMapping);
			}
			throw new ArgumentException("The object is not a CharacterMetric!");
		}
	}
}
