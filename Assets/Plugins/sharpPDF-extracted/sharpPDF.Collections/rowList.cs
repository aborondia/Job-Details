using System;
using System.Collections;
using sharpPDF.Tables;

namespace sharpPDF.Collections
{
	[Serializable]
	public class rowList : CollectionBase
	{
		public class rowEnumerator : IEnumerator
		{
			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public pdfTableRow Current => (pdfTableRow)baseEnumerator.Current;

			object IEnumerator.Current => baseEnumerator.Current;

			public rowEnumerator(rowList mappings)
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

		public pdfTableRow this[int index]
		{
			get
			{
				return (pdfTableRow)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public rowList()
		{
		}

		public rowList(rowList val)
		{
			AddRange(val);
		}

		public rowList(pdfTableRow[] val)
		{
			AddRange(val);
		}

		public int Add(pdfTableRow val)
		{
			return base.List.Add(val);
		}

		public void AddRange(pdfTableRow[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Add(val[i]);
			}
		}

		public void AddRange(rowList val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				Add(val[i]);
			}
		}

		public bool Contains(pdfTableRow val)
		{
			return base.List.Contains(val);
		}

		public void CopyTo(pdfTableRow[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(pdfTableRow val)
		{
			return base.List.IndexOf(val);
		}

		public void Insert(int index, pdfTableRow val)
		{
			base.List.Insert(index, val);
		}

		public new rowEnumerator GetEnumerator()
		{
			return new rowEnumerator(this);
		}

		public void Remove(pdfTableRow val)
		{
			base.List.Remove(val);
		}
	}
}
