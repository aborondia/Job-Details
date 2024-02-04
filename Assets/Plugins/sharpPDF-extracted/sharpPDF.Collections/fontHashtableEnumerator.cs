using System.Collections;
using sharpPDF.Fonts;

namespace sharpPDF.Collections
{
	public class fontHashtableEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private IDictionaryEnumerator innerEnumerator;

		public string Key => (string)innerEnumerator.Key;

		object IDictionaryEnumerator.Key => Key;

		public pdfAbstractFont Value => (pdfAbstractFont)innerEnumerator.Value;

		object IDictionaryEnumerator.Value => Value;

		public DictionaryEntry Entry => innerEnumerator.Entry;

		public object Current => innerEnumerator.Current;

		internal fontHashtableEnumerator(fontHashtable enumerable)
		{
			innerEnumerator = enumerable.InnerHash.GetEnumerator();
		}

		public void Reset()
		{
			innerEnumerator.Reset();
		}

		public bool MoveNext()
		{
			return innerEnumerator.MoveNext();
		}
	}
}
