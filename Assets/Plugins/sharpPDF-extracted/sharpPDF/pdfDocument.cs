using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using sharpPDF.Bookmarks;
using sharpPDF.Collections;
using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Exceptions;
using sharpPDF.Fonts;

namespace sharpPDF
{
	public class pdfDocument : IDisposable
	{
		private string _title;

		private string _author;

		private bool _openBookmark;

		private pdfHeader _header;

		private pdfInfo _info;

		private pdfOutlines _outlines;

		private pdfPageTree _pageTree;

		private pdfTrailer _trailer;

		private ArrayList _pages = null;

		private pdfPageMarker _pageMarker = null;

		private pdfPersistentPage _persistentPage = null;

		internal fontHashtable _fonts = new fontHashtable();

		internal imageHashtable _images = new imageHashtable();

		public pdfPageMarker pageMarker
		{
			set
			{
				_pageMarker = value;
			}
		}

		public pdfPersistentPage persistentPage => _persistentPage;

		public pdfPage this[int index] => (pdfPage)_pages[index];

		public pdfDocument(string title, string author)
		{
			_title = title;
			_author = author;
			_openBookmark = false;
			_outlines = new pdfOutlines();
			_pages = new ArrayList();
			_persistentPage = new pdfPersistentPage(this);
		}

		public pdfDocument(string title, string author, bool openBookmark)
		{
			_title = title;
			_author = author;
			_openBookmark = openBookmark;
			_outlines = new pdfOutlines();
			_pages = new ArrayList();
			_persistentPage = new pdfPersistentPage(this);
		}

		public void Dispose()
		{
			_header = null;
			_info = null;
			_outlines = null;
			_fonts = null;
			_pages = null;
			_pageTree = null;
			_trailer = null;
			_title = null;
			_author = null;
			_pageMarker = null;
			_persistentPage = null;
		}

		public void addTrueTypeFont(string fileName, string fontName)
		{
			if (!isFontLoaded(fontName))
			{
				_fonts.Add(fontName, pdfFontFactory.getFontObject(fileName, _fonts.Count + 1, documentFontType.csTrueTypeFont));
			}
		}

		internal void loadFont(string fontName, pdfAbstractFont fontObject)
		{
			_fonts.Add(fontName, fontObject);
		}

		internal bool isFontLoaded(string fontName)
		{
			return _fonts.ContainsKey(fontName);
		}

		public pdfAbstractFont getFontReference(predefinedFont fontType)
		{
			return getFontReference(pdfFontFactory.getPredefinedFontName(fontType));
		}

		public pdfAbstractFont getFontReference(string fontReference)
		{
			if (!isFontLoaded(fontReference))
			{
				if (!pdfFontFactory.isPredefinedFont(fontReference))
				{
					throw new pdfFontNotLoadedException();
				}
				_fonts.Add(fontReference, pdfFontFactory.getFontObject(fontReference, _fonts.Count + 1, documentFontType.csPredefinedfont));
			}
			return _fonts[fontReference];
		}

		public void addImageReference(string fileName, string imageName)
		{
			if (!isImageReferenceLoaded(imageName))
			{
				_images.Add(imageName, new pdfImageReference(fileName));
			}
		}

		public void addImageReference(Image imageObject, string imageName)
		{
			if (!isImageReferenceLoaded(imageName))
			{
				_images.Add(imageName, new pdfImageReference(imageObject));
			}
		}

		internal bool isImageReferenceLoaded(string imageName)
		{
			return _images.ContainsKey(imageName);
		}

		public pdfImageReference getImageReference(string imageName)
		{
			if (isImageReferenceLoaded(imageName))
			{
				return _images[imageName];
			}
			throw new pdfImageNotLoadedException();
		}

		public void createPDF(Stream outStream, Action<BufferedStream> useStream = null)
		{
			long num = 0L;
			initializeObjects();
			try
			{
				BufferedStream bufferedStream = new BufferedStream(outStream);
				num += writeToBuffer(bufferedStream, "%PDF-1.5" + Convert.ToChar(13) + Convert.ToChar(10));
				_trailer.addObject(num.ToString());
				num += writeToBuffer(bufferedStream, _header.getText());
				_trailer.addObject(num.ToString());
				num += writeToBuffer(bufferedStream, _info.getText());
				_trailer.addObject(num.ToString());
				num += writeToBuffer(bufferedStream, _outlines.getText());
				foreach (pdfBookmarkNode bookmark in _outlines.getBookmarks())
				{
					_trailer.addObject(num.ToString());
					num += writeToBuffer(bufferedStream, bookmark.getText());
				}
				foreach (pdfAbstractFont value in _fonts.Values)
				{
					_trailer.addObject(num.ToString());
					num += writeToBuffer(bufferedStream, value.getText());
					if (value is pdfTrueTypeFont)
					{
						_trailer.addObject(num.ToString());
						num += writeToBuffer(bufferedStream, ((pdfTrueTypeFont)value).getFontDescriptorText());
						_trailer.addObject(num.ToString());
						num += writeToBuffer(bufferedStream, ((pdfTrueTypeFont)value).getFontDescendantText());
						_trailer.addObject(num.ToString());
						num += writeToBuffer(bufferedStream, ((pdfTrueTypeFont)value).getToUnicodeText());
						_trailer.addObject(num.ToString());
						num += writeToBuffer(bufferedStream, ((pdfTrueTypeFont)value).getStreamText());
						num += writeToBuffer(bufferedStream, "stream" + Convert.ToChar(13) + Convert.ToChar(10));
						num += writeToBuffer(bufferedStream, ((pdfTrueTypeFont)value).subsetStream);
						num += writeToBuffer(bufferedStream, Convert.ToChar(13).ToString());
						num += writeToBuffer(bufferedStream, Convert.ToChar(10).ToString());
						num += writeToBuffer(bufferedStream, "endstream" + Convert.ToChar(13) + Convert.ToChar(10));
						num += writeToBuffer(bufferedStream, "endobj" + Convert.ToChar(13) + Convert.ToChar(10));
					}
				}
				_trailer.addObject(num.ToString());
				num += writeToBuffer(bufferedStream, _pageTree.getText());
				foreach (pdfPage page in _pages)
				{
					_trailer.addObject(num.ToString());
					num += writeToBuffer(bufferedStream, page.getText());
					foreach (pdfElement element in page.elements)
					{
						_trailer.addObject(num.ToString());
						num += writeToBuffer(bufferedStream, element.getText());
					}
				}
				foreach (pdfImageReference value2 in _images.Values)
				{
					_trailer.addObject(num.ToString());
					num += writeToBuffer(bufferedStream, value2.getText());
					num += writeToBuffer(bufferedStream, "stream" + Convert.ToChar(13) + Convert.ToChar(10));
					num += writeToBuffer(bufferedStream, value2.content);
					num += writeToBuffer(bufferedStream, Convert.ToChar(13).ToString());
					num += writeToBuffer(bufferedStream, Convert.ToChar(10).ToString());
					num += writeToBuffer(bufferedStream, "endstream" + Convert.ToChar(13) + Convert.ToChar(10));
					num += writeToBuffer(bufferedStream, "endobj" + Convert.ToChar(13) + Convert.ToChar(10));
				}
				_trailer.xrefOffset = num;
				num += writeToBuffer(bufferedStream, _trailer.getText());

				bufferedStream.Position = 0;
				if (!ReferenceEquals(useStream, null))
				{
					useStream.Invoke(bufferedStream);
				}

				bufferedStream.Flush();
				bufferedStream.Close();
				bufferedStream = null;
			}
			catch (IOException ex)
			{
				throw new pdfWritingErrorException("Errore nella scrittura del PDF", ex);
			}
		}

		public void createPDF(string outputFile)
		{
			try
			{
				FileStream fileStream = new FileStream(outputFile, FileMode.Create);
				createPDF(fileStream);
				fileStream.Close();
			}
			catch (IOException ex)
			{
				throw new pdfWritingErrorException("Errore nella scrittura del file", ex);
			}
			catch (pdfWritingErrorException ex2)
			{
				throw new pdfWritingErrorException("Errore nella scrittura del PDF", ex2);
			}
		}

		private void initializeObjects()
		{
			int num = 1;
			int count = _pages.Count;
			int num2 = 0;
			_header = new pdfHeader(_openBookmark);
			_header.objectIDHeader = 1;
			_header.objectIDInfo = 2;
			_header.objectIDOutlines = 3;
			_info = new pdfInfo(_title, _author);
			_info.objectIDInfo = 2;
			_outlines.objectIDOutlines = 3;
			num2 = 4;
			num2 = _outlines.initializeOutlines(num2);
			foreach (pdfAbstractFont value in _fonts.Values)
			{
				value.objectID = num2;
				num2++;
				if (value is pdfTrueTypeFont)
				{
					((pdfTrueTypeFont)value).descriptorObjectID = num2;
					num2++;
					((pdfTrueTypeFont)value).descendantObjectID = num2;
					num2++;
					((pdfTrueTypeFont)value).toUnicodeObjectID = num2;
					num2++;
					((pdfTrueTypeFont)value).streamObjectID = num2;
					num2++;
				}
			}
			_pageTree = new pdfPageTree();
			_pageTree.objectID = num2;
			_header.pageTreeID = num2;
			num2++;
			foreach (pdfPage page in _pages)
			{
				page.objectID = num2;
				page.pageTreeID = _pageTree.objectID;
				_pageTree.addPage(num2);
				num2++;
				if (_pageMarker != null)
				{
					page.addText(_pageMarker.getMarker(num, count), _pageMarker.coordX, _pageMarker.coordY, _pageMarker.fontType, _pageMarker.fontSize, _pageMarker.fontColor);
				}
				if (_persistentPage.elements.Count > 0)
				{
					page.elements.InsertRange(0, _persistentPage.elements);
				}
				foreach (pdfElement element in page.elements)
				{
					element.objectID = num2;
					num2++;
				}
				num++;
			}
			foreach (pdfImageReference value2 in _images.Values)
			{
				value2.ObjectID = num2;
				num2++;
			}
			_trailer = new pdfTrailer(num2 - 1);
		}

		public pdfPage addPage()
		{
			_pages.Add(new pdfPage(this));
			return (pdfPage)_pages[_pages.Count - 1];
		}

		public pdfPage addPage(predefinedPageSize predefinedSize)
		{
			_pages.Add(new pdfPage(predefinedSize, this));
			return (pdfPage)_pages[_pages.Count - 1];
		}

		public pdfPage addPage(int height, int width)
		{
			_pages.Add(new pdfPage(height, width, this));
			return (pdfPage)_pages[_pages.Count - 1];
		}

		public void addBookmark(pdfBookmarkNode Bookmark)
		{
			_outlines.addBookmark(Bookmark);
		}

		private long writeToBuffer(BufferedStream myBuffer, string stringContent)
		{
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			try
			{
				byte[] bytes = aSCIIEncoding.GetBytes(stringContent);
				myBuffer.Write(bytes, 0, bytes.Length);
				return bytes.Length;
			}
			catch (IOException ex)
			{
				throw new pdfBufferErrorException("Errore nella scrittura del Buffer", ex);
			}
		}

		private long writeToBuffer(BufferedStream myBuffer, byte[] byteContent)
		{
			try
			{
				myBuffer.Write(byteContent, 0, byteContent.Length);
				return byteContent.Length;
			}
			catch (IOException ex)
			{
				throw new pdfBufferErrorException("Errore nella scrittura del Buffer", ex);
			}
		}
	}
}
