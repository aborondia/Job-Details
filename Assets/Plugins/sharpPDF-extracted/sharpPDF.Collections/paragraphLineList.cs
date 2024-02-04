using System;
using System.Collections;
using sharpPDF.Elements;

namespace sharpPDF.Collections
{
	[Serializable]
	public class paragraphLineList : CollectionBase
	{
		public class paragraphLineEnumerator : IEnumerator
		{
			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public paragraphLine Current => (paragraphLine)baseEnumerator.Current;

			object IEnumerator.Current => baseEnumerator.Current;

			public paragraphLineEnumerator(paragraphLineList mappings)
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

		public paragraphLine this[int index]
		{
			get
			{
				return (paragraphLine)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public paragraphLineList()
		{
		}

		public paragraphLineList(paragraphLineList val)
		{
			AddRange(val);
		}

		public paragraphLineList(paragraphLine[] val)
		{
			AddRange(val);
		}

		public int Add(paragraphLine val)
		{
			return base.List.Add(val);
		}

		public void AddRange(paragraphLine[] val)
		{
			for (int i = 0; i < val.Length; i++)
			{
				Add(val[i]);
			}
		}

		public void AddRange(paragraphLineList val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				Add(val[i]);
			}
		}

		public bool Contains(paragraphLine val)
		{
			return base.List.Contains(val);
		}

		public void CopyTo(paragraphLine[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(paragraphLine val)
		{
			return base.List.IndexOf(val);
		}

		public void Insert(int index, paragraphLine val)
		{
			base.List.Insert(index, val);
		}

		public new paragraphLineEnumerator GetEnumerator()
		{
			return new paragraphLineEnumerator(this);
		}

		public void Remove(paragraphLine val)
		{
			base.List.Remove(val);
		}
	}
}
