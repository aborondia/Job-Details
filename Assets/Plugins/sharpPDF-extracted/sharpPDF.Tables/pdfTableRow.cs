using System;
using System.Collections;
using sharpPDF.Collections;
using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Exceptions;

namespace sharpPDF.Tables
{
	public class pdfTableRow : IEnumerable, ISeparable, ICloneable
	{
		protected columnList _cols = new columnList();

		protected int _rowHeight = 0;

		protected int _rowWidth = 0;

		protected pdfTableStyle _rowStyle = null;

		protected pdfTable _containerTable = null;

		protected predefinedVerticalAlignment _rowVerticalAlignment;

		protected int _startX = 0;

		protected int _startY = 0;

		internal pdfTable containerTable => _containerTable;

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

		public pdfTableColumn this[int index]
		{
			get
			{
				if (index < 0 || index >= _cols.Count)
				{
					throw new pdfBadColumnIndexException();
				}
				return _cols[index];
			}
		}

		public int columnsCount => _cols.Count;

		public int rowHeight
		{
			get
			{
				return _rowHeight;
			}
			set
			{
				_rowHeight = Math.Max(_rowHeight, value);
			}
		}

		public int rowWidth => _rowWidth;

		public pdfTableStyle rowStyle
		{
			get
			{
				return _rowStyle;
			}
			set
			{
				_rowStyle = value;
				foreach (pdfTableColumn col in _cols)
				{
					col.columnStyle = value;
				}
			}
		}

		public predefinedVerticalAlignment rowVerticalAlign
		{
			get
			{
				return _rowVerticalAlignment;
			}
			set
			{
				_rowVerticalAlignment = value;
				foreach (pdfTableColumn col in _cols)
				{
					col.columnVerticalAlign = value;
				}
			}
		}

		internal pdfTableRow(pdfTable containerTable)
		{
			_containerTable = containerTable;
			_rowStyle = containerTable.GetCurrentStyle();
			_rowVerticalAlignment = predefinedVerticalAlignment.csMiddle;
		}

		internal pdfTableRow(pdfTableHeader tableHeader)
			: this(tableHeader.containerTable)
		{
			foreach (pdfTableColumn item in tableHeader)
			{
				_cols.Add(new pdfTableColumn(this, item.columnWidth, item.columnAlign, item.columnVerticalAlign, _rowStyle));
				_cols[_cols.Count - 1].ColumnChanged += columnChanged;
			}
			_rowWidth = tableHeader.rowWidth;
			_rowHeight = _containerTable.cellpadding * 2;
		}

		public IEnumerator GetEnumerator()
		{
			return _cols.GetEnumerator();
		}

		internal void addColumn(pdfTableColumn column)
		{
			_cols.Add(column);
			column.ColumnChanged += columnChanged;
			_rowWidth = _containerTable.tableHeader.rowWidth;
		}

		internal int calculateWidth()
		{
			int num = 0;
			num = _containerTable.cellpadding * _cols.Count * 2;
			foreach (pdfTableColumn col in _cols)
			{
				num += col.columnWidth;
			}
			return num;
		}

		internal int calculateHeight()
		{
			int val = 0;
			foreach (pdfTableColumn col in _cols)
			{
				val = Math.Max(val, col.columnHeight + _containerTable.cellpadding * 2);
			}
			return Math.Max(val, _rowHeight);
		}

		public elementList GetBasicElements()
		{
			elementList elementList = new elementList();
			elementList.Add(new rectangleElement(_startX, _startY, _startX + _rowWidth, _startY - _rowHeight, _containerTable.borderColor, _rowStyle.bgColor, _containerTable.borderSize));
			foreach (pdfTableColumn col in _cols)
			{
				col.startX = _startX + _containerTable.cellpadding;
				col.startY = _startY - _containerTable.cellpadding;
				elementList.AddRange(col.GetBasicElements());
				_startX += col.columnWidth + _containerTable.cellpadding * 2;
				if (_cols.IndexOf(col) < _cols.Count - 1)
				{
					elementList.Add(new lineElement(_startX, _startY, _startX, _startY - rowHeight, _containerTable.borderSize, _containerTable.borderColor));
				}
				col.startX = 0;
				col.startY = 0;
			}
			return elementList;
		}

		protected void columnChanged(object sender, columnTableEventArgs e)
		{
			_rowHeight = Math.Max(_rowHeight, e.Column.columnHeight + _containerTable.cellpadding * 2);
		}

		public virtual object Clone()
		{
			pdfTableRow pdfTableRow2 = new pdfTableRow(_containerTable);
			pdfTableRow2.rowStyle = new pdfTableStyle(_rowStyle.fontReference, _rowStyle.fontSize, _rowStyle.fontColor, _rowStyle.bgColor);
			pdfTableRow2.rowVerticalAlign = _rowVerticalAlignment;
			foreach (pdfTableColumn col in _cols)
			{
				pdfTableRow2._cols.Add((pdfTableColumn)col.Clone());
			}
			pdfTableRow2.rowHeight = _rowHeight;
			pdfTableRow2._rowWidth = _rowWidth;
			return pdfTableRow2;
		}
	}
}
