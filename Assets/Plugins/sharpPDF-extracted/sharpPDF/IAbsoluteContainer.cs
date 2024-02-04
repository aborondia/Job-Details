using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;
using sharpPDF.PDFControls;

namespace sharpPDF
{
	public interface IAbsoluteContainer
	{
		void addImage(pdfImageReference imageReference, int X, int Y);

		void addImage(pdfImageReference imageReference, int X, int Y, int height, int width);

		void addText(string newText, int X, int Y, pdfAbstractFont fontReference, int fontSize);

		void addText(string newText, int X, int Y, pdfAbstractFont fontReference, int fontSize, pdfColor fontColor);

		void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth);

		void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, predefinedAlignment parAlign);

		void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, pdfColor textColor);

		void addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, pdfColor textColor, predefinedAlignment parAlign);

		string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight);

		string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, pdfColor textColor);

		string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, predefinedAlignment parAlign);

		string addParagraph(string newText, int x, int y, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, pdfColor textColor, predefinedAlignment parAlign);

		void drawLine(int X, int Y, int X1, int Y1);

		void drawLine(int X, int Y, int X1, int Y1, pdfColor lineColor);

		void drawLine(int X, int Y, int X1, int Y1, pdfColor lineColor, int lineWidth);

		void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle);

		void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle, int lineWidth);

		void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle, pdfColor lineColor);

		void drawLine(int X, int Y, int X1, int Y1, predefinedLineStyle lineStyle, pdfColor lineColor, int lineWidth);

		void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor);

		void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, int borderWidth);

		void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle borderStyle);

		void drawRectangle(int X, int Y, int X1, int Y1, pdfColor strokeColor, pdfColor fillColor, int borderWidth, predefinedLineStyle borderStyle);

		void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor);

		void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor, int borderWidth);

		void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle borderStyle);

		void drawCircle(int X, int Y, int ray, pdfColor strokeColor, pdfColor fillColor, predefinedLineStyle borderStyle, int borderWidth);

		void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open);

		void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open, pdfColor newColor);

		void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open, predefinedAnnotationStyle newStyle);

		void addAnnotation(string newContent, int newCoordX, int newCoordY, bool open, pdfColor newColor, predefinedAnnotationStyle newStyle);

		void addElement(pdfElement MyElement);

		void addControl(pdfControl MyControl);
	}
}
