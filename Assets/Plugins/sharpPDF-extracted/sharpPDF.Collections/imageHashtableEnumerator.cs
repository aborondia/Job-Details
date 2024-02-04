using System.Collections;
using sharpPDF.Elements;

namespace sharpPDF.Collections
{
	public class imageHashtableEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private IDictionaryEnumerator innerEnumerator;

		public string Key => (string)innerEnumerator.Key;

		object IDictionaryEnumerator.Key => Key;

		public pdfImageReference Value => (pdfImageReference)innerEnumerator.Value;

		object IDictionaryEnumerator.Value => Value;

		public DictionaryEntry Entry => innerEnumerator.Entry;

		public object Current => innerEnumerator.Current;

		internal imageHashtableEnumerator(imageHashtable enumerable)
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
