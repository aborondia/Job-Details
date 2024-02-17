using sharpPDF;
using sharpPDF.Fonts;
using sharpPDF.Enumerators;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JobDetails;
using System.IO;
using UnityEngine.Networking;

[RequireComponent(typeof(JobDetailsContentCreator))]
public class DocumentCreator : MonoBehaviour
{
    // Page = 612 x 792
    public static DocumentCreator Active;
    [SerializeField] private JobDetailsContentCreator jobDetailsContentCreator;
    [SerializeField] private string documentTitle = "Job Detail";
    [SerializeField] private string author = "N/A";
    [SerializeField] private int fontSize = 14;
    [SerializeField] private Color lineColor = Color.black;
    private pdfColor borderLinePDFColor;
    public pdfColor BorderLinePDFColor => borderLinePDFColor;
    [SerializeField] private Color rectangleColor = Color.blue;
    private pdfColor rectanglePDFColor;
    public pdfColor RectanglePDFColor => rectanglePDFColor;
    public int FontSize => fontSize;
    [SerializeField] private int borderLineWidth = 1;
    public int BorderLineWidth => borderLineWidth;
    private pdfDocument document;
    private byte[] fontReference;
    private pdfAbstractFont font;
    public pdfAbstractFont Font => font;
    private predefinedLineStyle lineStyle = predefinedLineStyle.csNormal;
    public predefinedLineStyle LineStyle => lineStyle;
    public static int YMargin = 20;
    public static int XMargin = 50;
    public static int DefaultPageWidth = 612;
    public static int DefaultPageHeight = 792;
    private bool sent = false;
    
    private void Awake()
    {
        if (Active != null)
        {
            GameObject.Destroy(Active);
        }

        Active = this;
        this.document = new pdfDocument(this.documentTitle, this.author);

#if UNITY_EDITOR
        this.font = this.document.getFontReference("Helvetica");
#else
                StartCoroutine(GetFontReference("Helvetica"));
#endif
        this.borderLinePDFColor = new pdfColor((int)this.lineColor.r, (int)this.lineColor.g, (int)this.lineColor.b);
        this.rectanglePDFColor = new pdfColor((int)this.rectangleColor.r, (int)this.rectangleColor.g, (int)this.rectangleColor.b);
    }

    private IEnumerator GetFontReference(string fontReference)
    {
        UnityWebRequest www = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, "Fonts", $"{fontReference}.afm"));

        yield return www.SendWebRequest();

        this.fontReference = www.downloadHandler.data;

        this.font = this.document.getFontReference(fontReference, this.fontReference);
    }

    public pdfDocument GetDocument(DetailsReport report)
    {
        foreach (JobDetail jobDetail in report.Details)
        {
            PDFPage detailPage = new PDFPage(this.document);
            List<string> timeContent = this.jobDetailsContentCreator.GetTimeContentText(jobDetail);
            List<string> paymentOptionsText = this.jobDetailsContentCreator.GetPaymentText();
            List<BorderLineWidth> paymentOptionBorders = this.jobDetailsContentCreator.GetPaymentBorders();
            List<BorderLineWidth> cleanerTagLineBorders = this.jobDetailsContentCreator.GetCleanerTagLineBorders();
            List<BorderLineWidth> cleanerLineBorders = this.jobDetailsContentCreator.GetCleanerLineBorders();

            this.jobDetailsContentCreator.StartCreatingPDFPage(detailPage);

            this.jobDetailsContentCreator.AddTextWithLine($"Date: {jobDetail.StartTime}");
            detailPage.OnTextLineAdded();

            this.jobDetailsContentCreator.AddTextWithLine($"Client Name: {jobDetail.ClientName}");
            detailPage.OnTextLineAdded();

            this.jobDetailsContentCreator.AddTextWithLine($"Client Address: {jobDetail.ClientAddress}");
            detailPage.OnTextLineAdded();

            this.jobDetailsContentCreator.AddMultiText(timeContent, true);
            detailPage.OnTextLineAdded();

            this.jobDetailsContentCreator.AddTextWithLine($"Job Type: {jobDetail.JobType}");
            detailPage.OnTextLineAdded(10);

            for (int i = 0; i < jobDetail.Cleaners.Count; i++)
            {
                if (i == 0)
                {
                    this.jobDetailsContentCreator.AddMultiText(jobDetail.GetCleanersContent(i), cleanerTagLineBorders);
                }
                else
                {
                    this.jobDetailsContentCreator.AddMultiText(jobDetail.GetCleanersContent(i), cleanerLineBorders);
                }

                detailPage.OnTextLineAdded();
            }

            detailPage.OnTextLineAdded();
            this.jobDetailsContentCreator.DrawJobDescriptionContainer();

            detailPage.OnTextLineAdded();
            this.jobDetailsContentCreator.AddMultiText(paymentOptionsText, paymentOptionBorders, true, (int)jobDetail.PaymentType);

            detailPage.OnTextLineAdded();
            this.jobDetailsContentCreator.AddParagraph("Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum Lorem Ipsum ");

            this.jobDetailsContentCreator.StopCreatingPDFPage();
        }

        // document.createPDF(@"C:\Users\MZ-admin\Desktop\Notes\test.pdf");

        Debug.Log("PDF Created!");
        return document;
    }
}
