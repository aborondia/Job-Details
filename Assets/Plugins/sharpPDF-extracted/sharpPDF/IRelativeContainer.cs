using sharpPDF.Elements;
using sharpPDF.Enumerators;
using sharpPDF.Fonts;

namespace sharpPDF
{
	public interface IRelativeContainer
	{
		void addImage(pdfImageReference imageReference);

		void addImage(pdfImageReference imageReference, int height, int width);

		void addText(string newText, pdfAbstractFont fontReference, int fontSize);

		void addText(string newText, pdfAbstractFont fontReference, int fontSize, pdfColor fontColor);

		void addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, predefinedAlignment parAlign);

		void addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, pdfColor textColor, predefinedAlignment parAlign);

		string addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, predefinedAlignment parAlign);

		string addParagraph(string newText, pdfAbstractFont fontReference, int fontSize, int lineHeight, int parWidth, int parHeight, pdfColor textColor, predefinedAlignment parAlign);
	}
}
