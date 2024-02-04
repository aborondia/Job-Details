using System;
using System.Collections;
using sharpPDF.Tables;

namespace sharpPDF.Collections
{
	[Serializable]
	public class columnList : CollectionBase
	{
		public class columnEnumerator : IEnumerator
		{
			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public pdfTableColumn Current => (pdfTableColumn)baseEnumerator.Current;

			object IEnumerator.Current => baseEnumerator.Current;

			public columnEnumerator(columnList mappings)
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

		public pdfTableColumn this[int index]
		{
			get
			{
				return (pdfTableColumn)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public columnList()
		{
		}

		public columnList(columnList val)
		{
			AddRange(val);
		}

		public columnList(pdfTableColumn[] val)
		{
			AddRange(val);
		}

		public int Add(pdfTableColumn val)
		{
			return base.List.Add(val);
		}

		public void AddRange(pdfTableColumn[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Add(val[i]);
			}
		}

		public void AddRange(columnList val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				Add(val[i]);
			}
		}

		public bool Contains(pdfTableColumn val)
		{
			return base.List.Contains(val);
		}

		public void CopyTo(pdfTableColumn[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(pdfTableColumn val)
		{
			return base.List.IndexOf(val);
		}

		public void Insert(int index, pdfTableColumn val)
		{
			base.List.Insert(index, val);
		}

		public new columnEnumerator GetEnumerator()
		{
			return new columnEnumerator(this);
		}

		public void Remove(pdfTableColumn val)
		{
			base.List.Remove(val);
		}
	}
}
