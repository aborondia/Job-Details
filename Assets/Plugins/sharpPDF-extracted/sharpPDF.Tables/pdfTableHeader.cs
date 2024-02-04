using sharpPDF.Enumerators;

namespace sharpPDF.Tables
{
	public class pdfTableHeader : pdfTableRow
	{
		protected bool _visible;

		public bool visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}

		internal event columnTableEventHandler ColumnAdded;

		internal pdfTableHeader(pdfTable Table, pdfTableStyle headerStyle)
			: base(Table)
		{
			_visible = true;
			_rowHeight = Table.cellpadding * 2;
			_rowWidth = Table.cellpadding * 2;
			_rowStyle = headerStyle;
		}

		public void addColumn(int columnWidth)
		{
			addColumn(columnWidth, predefinedAlignment.csLeft, predefinedVerticalAlignment.csMiddle);
		}

		public void addColumn(int columnWidth, predefinedAlignment columnAlign)
		{
			addColumn(columnWidth, columnAlign, predefinedVerticalAlignment.csMiddle);
		}

		public void addColumn(int columnWidth, predefinedAlignment columnAlign, predefinedVerticalAlignment columnVerticalAlign)
		{
			_cols.Add(new pdfTableColumn(this, columnWidth - _containerTable.cellpadding * 2, columnAlign, columnVerticalAlign, _rowStyle));
			_cols[_cols.Count - 1].ColumnChanged += base.columnChanged;
			_rowWidth = calculateWidth();
			if (this.ColumnAdded != null)
			{
				this.ColumnAdded(this, new columnTableEventArgs(_cols[_cols.Count - 1]));
			}
		}

		public override object Clone()
		{
			pdfTableHeader pdfTableHeader2 = new pdfTableHeader(_containerTable, new pdfTableStyle(_rowStyle.fontReference, _rowStyle.fontSize, _rowStyle.fontColor, _rowStyle.bgColor));
			pdfTableHeader2.rowVerticalAlign = _rowVerticalAlignment;
			pdfTableHeader2.visible = _visible;
			foreach (pdfTableColumn col in _cols)
			{
				pdfTableHeader2._cols.Add((pdfTableColumn)col.Clone());
			}
			pdfTableHeader2.rowHeight = _rowHeight;
			pdfTableHeader2._rowWidth = _rowWidth;
			return pdfTableHeader2;
		}
	}
}
