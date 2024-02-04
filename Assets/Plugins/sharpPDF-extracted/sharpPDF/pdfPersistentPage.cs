namespace sharpPDF
{
	public class pdfPersistentPage : pdfBasePage
	{
		public pdfPersistentPage(pdfDocument containerDoc)
			: base(containerDoc)
		{
		}

		~pdfPersistentPage()
		{
			_containerDoc = null;
			_elements = null;
		}
	}
}
