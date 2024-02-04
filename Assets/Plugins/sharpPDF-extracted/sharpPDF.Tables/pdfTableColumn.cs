using System;
using sharpPDF.Collections;
using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;

namespace sharpPDF.Tables
{
	public class pdfTableColumn : IRelativeContainer, ISeparable, ICloneable
	{
		protected int _currentX = 0;

		protected int _currentY = 0;

		protected int _columnWidth = 0;

		protected int _columnHeight = 0;

		protected predefinedAlignment _columnAlign;

		protected predefinedVerticalAlignment _columnVerticalAlign;

		protected pdfTableStyle _columnStyle = null;

		protected elementList _content = new elementList();

		protected pdfTableRow _containerRow;

		protected int _startX = 0;

		protected int _startY = 0;

		public int columnWidth => _columnWidth;

		public int columnHeight
		{
			get
			{
				return Math.Max(_columnHeight, _currentY);
			}
			set
			{
				if (value - _containerRow.containerTable.cellpadding * 2 > _columnHeight)
				{
					_columnHeight = value - _containerRow.containerTable.cellpadding * 2;
					if (this.ColumnChanged != null)
					{
						this.ColumnChanged(this, new columnTableEventArgs(this));
					}
				}
			}
		}

		public predefinedAlignment columnAlign
		{
			get
			{
				return _columnAlign;
			}
			set
			{
				_columnAlign = value;
			}
		}

		public predefinedVerticalAlignment columnVerticalAlign
		{
			get
			{
				return _columnVerticalAlign;
			}
			set
			{
				_columnVerticalAlign = value;
			}
		}

		public pdfTableStyle columnStyle
		{
			get
			{
				return _columnStyle;
			}
			set
			{
				_columnStyle = value;
			}
		}

		internal pdfTableRow containerRow => _containerRow;

		internal int startX
		{
			get
			{
				return _startX;
			}
			set
			{
				_startX = value;
			}
		}

		internal int startY
		{
			get
			{
				return _startY;
			}
			set
			{
				_startY = value;
			}
		}

		internal event columnTableEventHandler ColumnChanged;

		internal pdfTableColumn(pdfTableRow containerRow, int columnWidth, predefinedAlignment columnAlign, pdfTableStyle columnStyle)
			: this(containerRow, columnWidth, columnAlign, predefinedVerticalAlignment.csMiddle, columnStyle)
		{
		}

		internal pdfTableColumn(pdfTableRow containerRow, int columnWidth, predefinedAlignment columnAlign, predefinedVerticalAlignment columnVerticalAlign, pdfTableStyle columnStyle)
		{
			_containerRow = containerRow;
			_columnWidth = columnWidth;
			_columnAlign = columnAlign;
			_columnVerticalAlign = columnVerticalAlign;
			_columnStyle = columnStyle;
		}

		public void addText(string newText)
		{
			addText(newText, _columnStyle.fontReference, _columnStyle.fontSize, _columnStyle.fontColor);
		}

		public void addParagraph(string newText, int lineHeight, predefinedAlignment parAlign)
		{
			addParagraph(newText, _columnStyle.fontReference, _columnStyle.fontSize, lineHeight, _columnWidth, _columnStyle.fontColor, parAlign);
		}

		public string addParagraph(string newText, int lineHeight, predefinedAlignment parAlign, int parHeight)
		{
			return addParagraph(newText, _columnStyle.fontReference, _columnStyle.fontSize, lineHeight, _columnWidth, parHeight, _columnStyle.fontColor, parAlign);
		}

		public void addImage(pdfImageReference imageReference)
		{
			addImage(imageReference, imageReference.height, imageReference.width);
		}

		public void addImage(pdfImageReference imageReference, int height, int width)
		{
			if (width > _columnWidth)
			{
				height = Convert.ToInt32(Math.Round((double)_columnWidth * (double)height / (double)width));
				width = _columnWidth;
			}
			imageElement imageElement = new imageElement(imageReference, _currentX, _currentY + height, height, width);
			_content.Add(imageElement);
			_currentY += imageElement.height;
			if (this.ColumnChanged != null)
			{
				this.ColumnChanged(this, new columnTableEventArgs(this));
			}
			imageElement = null;
		}

		public void addText(string newText, pdfAbstractFont fontReference, int fontSize)
		{
			addText(newText, fontReference, fontSize, _columnStyle.fontColor);
		}

		public void addText(string newText, pdfAbstractFont fontReference, int fontSize, pdfColor fontColor)
		{
			int wordWidth = fontReference.getWordWidth(newText, fontSize);
			if (wordWidth > _columnWidth)
			{
				_content.Add(new textElement(fontReference.cropWord(newText, _columnWidth, fontSize), fontSize, fontReference, _currentX, _currentY + fontReference.fontDefinition.fontHeight * fontSize, fontColor));
			}
			else
			{
				_content.Add(new textElement(newText, fontSize, fontReference, _currentX, _currentY + fontReference.fontDefinition.fontHeight * fontSize, fontColor));
			}
			_currentY += _content[_content.Count - 1].height;
			if (this.ColumnChanged != null)
			{
				this.ColumnChanged(this, new columnTableEventArgs(this));
			}
		}

		public void addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, predefinedAlignment parAlign)
		{
			addParagraph(newText, fontReference, fontSize, lineHeight, parWidth, _columnStyle.fontColor, parAlign);
		}

		public void addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, pdfColor textColor, predefinedAlignment parAlign)
		{
			if (parWidth > _columnWidth)
			{
				parWidth = _columnWidth;
			}
			paragraphElement paragraphElement = new paragraphElement(textAdapter.formatParagraph(ref newText, fontSize, fontReference, parWidth, 0, lineHeight, parAlign), parWidth, lineHeight, fontSize, fontReference, _currentX, _currentY, textColor);
			_content.Add(paragraphElement);
			_currentY += paragraphElement.content.Count * lineHeight;
			if (this.ColumnChanged != null)
			{
				this.ColumnChanged(this, new columnTableEventArgs(this));
			}
			paragraphElement = null;
		}

		public string addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, predefinedAlignment parAlign)
		{
			return addParagraph(newText, fontReference, fontSize, lineHeight, parWidth, parHeight, _columnStyle.fontColor, parAlign);
		}

		public string addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, pdfColor textColor, predefinedAlignment parAlign)
		{
			if (parWidth > _columnWidth)
			{
				parWidth = _columnWidth;
			}
			paragraphElement paragraphElement = new paragraphElement(new paragraphLineList(textAdapter.formatParagraph(ref newText, fontSize, fontReference, parWidth, Convert.ToInt32(Math.Floor((double)parHeight / (double)lineHeight)), lineHeight, parAlign)), parWidth, lineHeight, fontSize, fontReference, _currentX, _currentY, textColor);
			_content.Add(paragraphElement);
			_currentY += paragraphElement.height;
			if (this.ColumnChanged != null)
			{
				this.ColumnChanged(this, new columnTableEventArgs(this));
			}
			paragraphElement = null;
			return newText;
		}

		public void insertBreak(int brHeight)
		{
			_currentY += brHeight;
		}

		public elementList GetBasicElements()
		{
			elementList elementList = new elementList(_content);
			foreach (pdfElement item in elementList)
			{
				switch (_columnAlign)
				{
				default:
					item.coordX += _startX;
					break;
				case predefinedAlignment.csCenter:
					item.coordX += _startX + Convert.ToInt32(Math.Round((double)(_columnWidth - item.width) / 2.0));
					break;
				case predefinedAlignment.csRight:
					item.coordX += _startX + (_columnWidth - item.width);
					break;
				}
				switch (_columnVerticalAlign)
				{
				default:
					item.coordY = _startY - item.coordY;
					break;
				case predefinedVerticalAlignment.csBottom:
					item.coordY = _startY - item.coordY - (_containerRow.rowHeight - _currentY - _containerRow.containerTable.cellpadding * 2);
					break;
				case predefinedVerticalAlignment.csMiddle:
					item.coordY = _startY - item.coordY - Convert.ToInt32(Math.Round(((double)_containerRow.rowHeight - (double)_currentY - (double)_containerRow.containerTable.cellpadding * 2.0) / 2.0));
					break;
				}
			}
			if (!_containerRow.rowStyle.Equals(_columnStyle))
			{
				elementList.Insert(0, new rectangleElement(_startX - _containerRow.containerTable.cellpadding, _startY + _containerRow.containerTable.cellpadding, _startX + _columnWidth + _containerRow.containerTable.cellpadding, _startY + _containerRow.containerTable.cellpadding - _containerRow.rowHeight, _containerRow.containerTable.borderColor, _columnStyle.bgColor, _containerRow.containerTable.borderSize));
			}
			return elementList;
		}

		public object Clone()
		{
			pdfTableColumn pdfTableColumn2 = new pdfTableColumn(_containerRow, _columnWidth, _columnAlign, _columnVerticalAlign, _columnStyle);
			pdfTableColumn2._columnHeight = columnHeight;
			pdfTableColumn2._currentX = _currentX;
			pdfTableColumn2._currentY = _currentY;
			foreach (pdfElement item in _content)
			{
				pdfTableColumn2._content.Add((pdfElement)item.Clone());
			}
			return pdfTableColumn2;
		}
	}
}
