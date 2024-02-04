using System.Collections;
using sharpPDF.Collections;
using sharpPDF.Enumerators;
using sharpPDF.Exceptions;

namespace sharpPDF.Tables
{
	public class pdfTable : pdfPositionableObject, IEnumerable, ISeparable
	{
		protected pdfDocument _containerDocument;

		protected pdfTableHeader _tableHeader;

		protected pdfTableStyle _rowStyle;

		protected pdfTableStyle _alternateRowStyle;

		protected bool _isAlternateStyle = false;

		protected rowList _rows = new rowList();

		protected int _borderSize;

		protected pdfColor _borderColor;

		protected int _cellpadding;

		public int borderSize
		{
			get
			{
				return _borderSize;
			}
			set
			{
				_borderSize = value;
			}
		}

		public pdfColor borderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
			}
		}

		public pdfTableHeader tableHeader => _tableHeader;

		public pdfTableStyle rowStyle => _rowStyle;

		public pdfTableStyle alternateRowStyle => _alternateRowStyle;

		public int cellpadding => _cellpadding;

		public pdfTableRow this[int index]
		{
			get
			{
				if (index < 0 || index >= _rows.Count)
				{
					throw new pdfBadRowIndexException();
				}
				return _rows[index];
			}
		}

		public int rowsCount => _rows.Count;

		internal pdfDocument containerDocument => _containerDocument;

		public pdfTable(pdfDocument containerDocument)
			: this(containerDocument, 1, pdfColor.Black, 5, new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White))
		{
		}

		public pdfTable(pdfDocument containerDocument, int border, pdfColor borderColor)
			: this(containerDocument, border, borderColor, 5, new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White))
		{
		}

		public pdfTable(pdfDocument containerDocument, int cellpadding)
			: this(containerDocument, 1, pdfColor.Black, cellpadding, new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White))
		{
		}

		public pdfTable(pdfDocument containerDocument, int border, pdfColor borderColor, int cellpadding)
			: this(containerDocument, border, borderColor, cellpadding, new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White))
		{
		}

		public pdfTable(pdfDocument containerDocument, int border, pdfColor borderColor, int cellpadding, pdfTableStyle headerStyle)
			: this(containerDocument, border, borderColor, cellpadding, headerStyle, new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White), new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White))
		{
		}

		public pdfTable(pdfDocument containerDocument, int border, pdfColor borderColor, int cellpadding, pdfTableStyle headerStyle, pdfTableStyle rowStyle)
			: this(containerDocument, border, borderColor, cellpadding, headerStyle, rowStyle, new pdfTableStyle(containerDocument.getFontReference(predefinedFont.csHelvetica), 10, pdfColor.Black, pdfColor.White))
		{
		}

		public pdfTable(pdfDocument containerDocument, int border, pdfColor borderColor, int cellpadding, pdfTableStyle headerStyle, pdfTableStyle rowStyle, pdfTableStyle alternateRowStyle)
		{
			_containerDocument = containerDocument;
			_tableHeader = new pdfTableHeader(this, headerStyle);
			_tableHeader.ColumnAdded += columnAdded;
			_rowStyle = rowStyle;
			_alternateRowStyle = alternateRowStyle;
			_borderSize = border;
			_borderColor = borderColor;
			_cellpadding = cellpadding;
		}

		public pdfTableRow createRow()
		{
			return new pdfTableRow(_tableHeader);
		}

		public void addRow(pdfTableRow newRow)
		{
			_rows.Add(newRow);
		}

		public IEnumerator GetEnumerator()
		{
			return _rows.GetEnumerator();
		}

		public elementList GetBasicElements()
		{
			int startX = _coordX;
			int num = _coordY;
			elementList elementList = new elementList();
			if (_tableHeader.visible)
			{
				_tableHeader.startX = startX;
				_tableHeader.startY = num;
				elementList.AddRange(_tableHeader.GetBasicElements());
				_tableHeader.startX = 0;
				_tableHeader.startY = 0;
			}
			num -= _tableHeader.rowHeight;
			foreach (pdfTableRow row in _rows)
			{
				row.startX = startX;
				row.startY = num;
				elementList.AddRange(row.GetBasicElements());
				row.startX = 0;
				row.startY = 0;
				num -= row.rowHeight;
			}
			return elementList;
		}

		public pdfTable CropTable(int tabHeight)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			pdfTable pdfTable2 = null;
			if (_tableHeader.visible)
			{
				tabHeight -= tableHeader.rowHeight;
			}
			while (num < _rows.Count && num2 <= tabHeight && !flag)
			{
				if (_rows[num].rowHeight > tabHeight)
				{
					throw new pdfBadRowHeightException();
				}
				if (_rows[num].rowHeight <= tabHeight - num2)
				{
					num2 += _rows[num].rowHeight;
					num++;
				}
				else
				{
					flag = true;
				}
			}
			if (num < _rows.Count)
			{
				pdfTable2 = new pdfTable(_containerDocument);
				pdfTable2._borderColor = _borderColor;
				pdfTable2._borderSize = _borderSize;
				pdfTable2._cellpadding = _cellpadding;
				pdfTable2._rowStyle = _rowStyle;
				pdfTable2._tableHeader = (pdfTableHeader)_tableHeader.Clone();
				pdfTable2._rowStyle = _rowStyle;
				pdfTable2._alternateRowStyle = _alternateRowStyle;
				while (num < _rows.Count)
				{
					pdfTable2._rows.Add(_rows[num]);
					_rows.RemoveAt(num);
				}
			}
			return pdfTable2;
		}

		internal pdfTableStyle GetCurrentStyle()
		{
			if (_isAlternateStyle && _alternateRowStyle != null)
			{
				_isAlternateStyle = false;
				return _alternateRowStyle;
			}
			_isAlternateStyle = true;
			return _rowStyle;
		}

		protected void columnAdded(object sender, columnTableEventArgs e)
		{
			foreach (pdfTableRow row in _rows)
			{
				row.addColumn(new pdfTableColumn(row, e.Column.columnWidth, e.Column.columnAlign, e.Column.columnVerticalAlign, row.rowStyle));
			}
		}
	}
}
