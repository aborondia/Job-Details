using System;
using System.Collections;
using sharpPDF.Collections;
using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;
using sharpPDF.PDFControls;
using sharpPDF.Tables;

namespace sharpPDF
{
	public abstract class pdfBasePage : IAbsoluteContainer
	{
		protected ArrayList _elements;

		protected pdfDocument _containerDoc;

		internal ArrayList elements => _elements;

		internal pdfBasePage(pdfDocument Container)
		{
			_containerDoc = Container;
			_elements = new ArrayList();
		}

		public void addImage(pdfImageReference imageReference, int X, int Y)
		{
			imageElement value = new imageElement(imageReference, X, Y);
			_elements.Add(value);
			value = null;
		}

		public void addImage(pdfImageReference imageReference, int X, int Y, int height, int width)
		{
			imageElement value = new imageElement(imageReference, X, Y, height, width);
			_elements.Add(value);
			value = null;
		}

		public void addText(string newText, int X, int Y, pdfAbstractFont fontReference, int fontSize)
		{
			addText(newText, X, Y, fontReference, fontSize, pdfColor.Black);
		}

		public void addText(string newText, int X, int Y, pdfAbstractFont fontReference, int fontSize, pdfColor fontColor)
		{
			_elements.Add(new textElement(newText, fontSize, fontReference, X, Y, fontColor));
		}

		public void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth)
		{
			addParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, pdfColor.Black, predefinedAlignment.csLeft);
		}

		public void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, predefinedAlignment parAlign)
		{
			addParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, pdfColor.Black, parAlign);
		}

		public void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, pdfColor textColor)
		{
			addParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, textColor, predefinedAlignment.csLeft);
		}

		public void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, pdfColor textColor, predefinedAlignment parAlign)
		{
			_elements.Add(new paragraphElement(textAdapter.formatParagraph(ref newText, fontSize, fontReference, parWidth, 0, lineHeight, parAlign), parWidth, lineHeight, fontSize, fontReference, x, y, textColor));
		}

		public string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight)
		{
			return addParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, parHeight, pdfColor.Black, predefinedAlignment.csLeft);
		}

		public string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, pdfColor textColor)
		{
			return addParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, parHeight, textColor, predefinedAlignment.csLeft);
		}

		public string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, predefinedAlignment parAlign)
		{
			return addParagraph(newText, x, y, fontReference, fontSize, lineHeight, parWidth, parHeight, pdfColor.Black, parAlign);
		}

		public string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, pdfColor textColor, predefinedAlignment parAlign)
		{
			_elements.Add(new paragraphElement(new paragraphLineList(textAdapter.formatParagraph(ref newText, fontSize, fontReference, parWidth, Convert.ToInt32(Math.Floor((double)parHeight / (double)lineHeight)), lineHeight, parAlign)), parWidth, lineHeight, fontSize, fontReference, x, y, textColor));
			return newText;
		}

		public void drawLine(int X, int Y, int X1, int Y1)
		{
			drawLine(X, Y, X1, Y1, predefinedLineStyle.csNormal, pdfColor.Black, 1);
		}

		public void drawLine(int X, int Y, int X1, int Y1, pdfColor lineColor)
		{
			drawLine(X, Y, X1, Y1, predefinedLineStyle.csNormal, lineColor, 1);
		}

		public void drawLine(int X, int Y, int X1, int Y1, pdfColor lineColor, int lineWidth)
		{
			drawLine(X, Y, X1, Y1, predefinedLineStyle.csNormal, lineColor, lineWidth);
		}

		public void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle)
		{
			drawLine(X, Y, X1, Y1, lineStyle, pdfColor.Black, 1);
		}

		public void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle, int lineWidth)
		{
			drawLine(X, Y, X1, Y1, lineStyle, pdfColor.Black, lineWidth);
		}

		public void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle, pdfColor lineColor)
		{
			drawLine(X, Y, X1, Y1, lineStyle, lineColor, 1);
		}

		public void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle, pdfColor lineColor, int lineWidth)
		{
			_elements.Add(new lineElement(X, Y, X1, Y1, lineWidth, lineStyle, lineColor));
		}

		public void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor)
		{
			drawRectangle(X, Y, X1, Y1, strokeColor, fillColor, 1, predefinedLineStyle.csNormal);
		}

		public void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, int borderWidth)
		{
			drawRectangle(X, Y, X1, Y1, strokeColor, fillColor, borderWidth, predefinedLineStyle.csNormal);
		}

		public void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle borderStyle)
		{
			drawRectangle(X, Y, X1, Y1, strokeColor, fillColor, 1, borderStyle);
		}

		public void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, int borderWidth, predefinedLineStyle borderStyle)
		{
			_elements.Add(new rectangleElement(X, Y, X1, Y1, strokeColor, fillColor, borderWidth, borderStyle));
		}

		public void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor)
		{
			drawCircle(X, Y, ray, strokeColor, fillColor, predefinedLineStyle.csNormal, 1);
		}

		public void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor, int borderWidth)
		{
			drawCircle(X, Y, ray, strokeColor, fillColor, predefinedLineStyle.csNormal, borderWidth);
		}

		public void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle borderStyle)
		{
			drawCircle(X, Y, ray, strokeColor, fillColor, borderStyle, 1);
		}

		public void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle borderStyle, int borderWidth)
		{
			_elements.Add(new circleElement(X, Y, ray, strokeColor, fillColor, borderWidth, borderStyle));
		}

		public void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open)
		{
			addAnnotation(newContent, newCoordX, newCoordY, open, pdfColor.LightGray, predefinedAnnotationStyle.csComment);
		}

		public void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open, pdfColor newColor)
		{
			addAnnotation(newContent, newCoordX, newCoordY, open, newColor, predefinedAnnotationStyle.csComment);
		}

		public void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open, predefinedAnnotationStyle newStyle)
		{
			addAnnotation(newContent, newCoordX, newCoordY, open, pdfColor.LightGray, newStyle);
		}

		public void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open, pdfColor newColor, predefinedAnnotationStyle newStyle)
		{
			_elements.Add(new annotationElement(newContent, newCoordX, newCoordY, newColor, newStyle, open));
		}

		public void addControl(pdfControl MyControl)
		{
			_elements.AddRange(MyControl.GetBasicElements());
		}

		public void addElement(pdfElement MyElement)
		{
			_elements.Add(MyElement);
		}

		public void addTable(pdfTable myTable)
		{
			_elements.AddRange(myTable.GetBasicElements());
		}

		public pdfTable addTable(pdfTable myTable, int tabHeight)
		{
			pdfTable result = myTable.CropTable(tabHeight);
			_elements.AddRange(myTable.GetBasicElements());
			return result;
		}
	}
}
