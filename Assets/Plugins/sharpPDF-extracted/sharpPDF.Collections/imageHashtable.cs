using System;
using System.Collections;
using sharpPDF.Elements;

namespace sharpPDF.Collections
{
	public class imageHashtable : IDictionary, ICollection, IEnumerable, ICloneable
	{
		protected Hashtable innerHash;

		public bool IsReadOnly => innerHash.IsReadOnly;

		public pdfImageReference this[string key]
		{
			get
			{
				return (pdfImageReference)innerHash[key];
			}
			set
			{
				innerHash[key] = value;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this[(string)key];
			}
			set
			{
				this[(string)key] = (pdfImageReference)value;
			}
		}

		public ICollection Values => innerHash.Values;

		public ICollection Keys => innerHash.Keys;

		public bool IsFixedSize => innerHash.IsFixedSize;

		public bool IsSynchronized => innerHash.IsSynchronized;

		public int Count => innerHash.Count;

		public object SyncRoot => innerHash.SyncRoot;

		internal Hashtable InnerHash => innerHash;

		public imageHashtable()
		{
			innerHash = new Hashtable();
		}

		public imageHashtable(imageHashtable original)
		{
			innerHash = new Hashtable(original.innerHash);
		}

		public imageHashtable(IDictionary dictionary)
		{
			innerHash = new Hashtable(dictionary);
		}

		public imageHashtable(int capacity)
		{
			innerHash = new Hashtable(capacity);
		}

		public imageHashtable(IDictionary dictionary, float loadFactor)
		{
			innerHash = new Hashtable(dictionary, loadFactor);
		}

		public imageHashtable(IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(codeProvider, comparer);
		}

		public imageHashtable(int capacity, int loadFactor)
		{
			innerHash = new Hashtable(capacity, loadFactor);
		}

		public imageHashtable(IDictionary dictionary, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(dictionary, codeProvider, comparer);
		}

		public imageHashtable(int capacity, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(capacity, codeProvider, comparer);
		}

		public imageHashtable(IDictionary dictionary, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(dictionary, loadFactor, codeProvider, comparer);
		}

		public imageHashtable(int capacity, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(capacity, loadFactor, codeProvider, comparer);
		}

		public imageHashtableEnumerator GetEnumerator()
		{
			return new imageHashtableEnumerator(this);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new imageHashtableEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Remove(string key)
		{
			innerHash.Remove(key);
		}

		void IDictionary.Remove(object key)
		{
			Remove((string)key);
		}

		public bool Contains(string key)
		{
			return innerHash.Contains(key);
		}

		bool IDictionary.Contains(object key)
		{
			return Contains((string)key);
		}

		public void Clear()
		{
			innerHash.Clear();
		}

		public void Add(string key, pdfImageReference value)
		{
			innerHash.Add(key, value);
		}

		void IDictionary.Add(object key, object value)
		{
			Add((string)key, (pdfImageReference)value);
		}

		public void CopyTo(Array array, int index)
		{
			innerHash.CopyTo(array, index);
		}

		public imageHashtable Clone()
		{
			imageHashtable imageHashtable2 = new imageHashtable();
			imageHashtable2.innerHash = (Hashtable)innerHash.Clone();
			return imageHashtable2;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public bool ContainsKey(string key)
		{
			return innerHash.ContainsKey(key);
		}

		public bool ContainsValue(pdfImageReference value)
		{
			return innerHash.ContainsValue(value);
		}

		public static imageHashtable Synchronized(imageHashtable nonSync)
		{
			imageHashtable imageHashtable2 = new imageHashtable();
			imageHashtable2.innerHash = Hashtable.Synchronized(nonSync.innerHash);
			return imageHashtable2;
		}
	}
}
