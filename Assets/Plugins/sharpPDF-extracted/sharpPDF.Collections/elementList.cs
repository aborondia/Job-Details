using System;
using System.Collections;
using sharpPDF.Elements;

namespace sharpPDF.Collections
{
	[Serializable]
	public class elementList : CollectionBase
	{
		public class elementEnumerator : IEnumerator
		{
			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public pdfElement Current => (pdfElement)baseEnumerator.Current;

			object IEnumerator.Current => baseEnumerator.Current;

			public elementEnumerator(elementList mappings)
			{
				temp = mappings;
				baseEnumerator = temp.GetEnumerator();
			}

			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				baseEnumerator.Reset();
			}
		}

		public pdfElement this[int index]
		{
			get
			{
				return (pdfElement)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public elementList()
		{
		}

		public elementList(elementList val)
		{
			AddRange(val);
		}

		public elementList(pdfElement[] val)
		{
			AddRange(val);
		}

		public int Add(pdfElement val)
		{
			return base.List.Add(val);
		}

		public void AddRange(pdfElement[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Add(val[i]);
			}
		}

		public void AddRange(elementList val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				Add(val[i]);
			}
		}

		public bool Contains(pdfElement val)
		{
			return base.List.Contains(val);
		}

		public void CopyTo(pdfElement[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(pdfElement val)
		{
			return base.List.IndexOf(val);
		}

		public void Insert(int index, pdfElement val)
		{
			base.List.Insert(index, val);
		}

		public new elementEnumerator GetEnumerator()
		{
			return new elementEnumerator(this);
		}

		public void Remove(pdfElement val)
		{
			base.List.Remove(val);
		}
	}
}
