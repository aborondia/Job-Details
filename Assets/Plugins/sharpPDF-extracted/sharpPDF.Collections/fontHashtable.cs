using System;
using System.Collections;
using sharpPDF.Fonts;

namespace sharpPDF.Collections
{
	public class fontHashtable : IDictionary, ICollection, IEnumerable, ICloneable
	{
		protected Hashtable innerHash;

		public bool IsReadOnly => innerHash.IsReadOnly;

		public pdfAbstractFont this[string key]
		{
			get
			{
				return (pdfAbstractFont)innerHash[key];
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
				this[(string)key] = (pdfAbstractFont)value;
			}
		}

		public ICollection Values => innerHash.Values;

		public ICollection Keys => innerHash.Keys;

		public bool IsFixedSize => innerHash.IsFixedSize;

		public bool IsSynchronized => innerHash.IsSynchronized;

		public int Count => innerHash.Count;

		public object SyncRoot => innerHash.SyncRoot;

		internal Hashtable InnerHash => innerHash;

		public fontHashtable()
		{
			innerHash = new Hashtable();
		}

		public fontHashtable(fontHashtable original)
		{
			innerHash = new Hashtable(original.innerHash);
		}

		public fontHashtable(IDictionary dictionary)
		{
			innerHash = new Hashtable(dictionary);
		}

		public fontHashtable(int capacity)
		{
			innerHash = new Hashtable(capacity);
		}

		public fontHashtable(IDictionary dictionary, float loadFactor)
		{
			innerHash = new Hashtable(dictionary, loadFactor);
		}

		public fontHashtable(IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(codeProvider, comparer);
		}

		public fontHashtable(int capacity, int loadFactor)
		{
			innerHash = new Hashtable(capacity, loadFactor);
		}

		public fontHashtable(IDictionary dictionary, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(dictionary, codeProvider, comparer);
		}

		public fontHashtable(int capacity, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(capacity, codeProvider, comparer);
		}

		public fontHashtable(IDictionary dictionary, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(dictionary, loadFactor, codeProvider, comparer);
		}

		public fontHashtable(int capacity, float loadFactor, IHashCodeProvider codeProvider, IComparer comparer)
		{
			innerHash = new Hashtable(capacity, loadFactor, codeProvider, comparer);
		}

		public fontHashtableEnumerator GetEnumerator()
		{
			return new fontHashtableEnumerator(this);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new fontHashtableEnumerator(this);
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

		public void Add(string key, pdfAbstractFont value)
		{
			innerHash.Add(key, value);
		}

		void IDictionary.Add(object key, object value)
		{
			Add((string)key, (pdfAbstractFont)value);
		}

		public void CopyTo(Array array, int index)
		{
			innerHash.CopyTo(array, index);
		}

		public fontHashtable Clone()
		{
			fontHashtable fontHashtable2 = new fontHashtable();
			fontHashtable2.innerHash = (Hashtable)innerHash.Clone();
			return fontHashtable2;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public bool ContainsKey(string key)
		{
			return innerHash.ContainsKey(key);
		}

		public bool ContainsValue(pdfAbstractFont value)
		{
			return innerHash.ContainsValue(value);
		}

		public static fontHashtable Synchronized(fontHashtable nonSync)
		{
			fontHashtable fontHashtable2 = new fontHashtable();
			fontHashtable2.innerHash = Hashtable.Synchronized(nonSync.innerHash);
			return fontHashtable2;
		}
	}
}
