using sharpPDF;
using sharpPDF.Fonts;
using sharpPDF.Enumerators;
using UnityEngine;
using System;
using System.Collections.Generic;
using JobDetails;
using DC = DocumentCreator;
using BorderLineType = BorderLineWidthCollection.BorderLineTypeEnum;

public class JobDetailsContentCreator : MonoBehaviour
{
    private PDFPage currentPDFPage;

    #region Setters

    public void StartCreatingPDFPage(PDFPage pdfPage)
    {
        this.currentPDFPage = pdfPage;

        pdfPage.SetNewPageCoordinates();
    }

    public void StopCreatingPDFPage()
    {
        this.currentPDFPage = null;
    }

    #endregion

    #region Getters

    public List<string> GetPaymentOptionsText()
    {
        List<string> paymentOptionsText = new List<string> { "Cash", "Cheque", "No Payment Method", "Premium", };

        return paymentOptionsText;
    }

    public List<string> GetTimeContentText(JobDetail jobDetail)
    {
        return new List<string>
        {
            $"Start Time: {jobDetail.StartTime}",
            $"Finish Time: {jobDetail.FinishTime}"
        };
    }

    public List<string> GetPaymentText()
    {
        return new List<string>
        {
            "Cash:",
            "Cheque:",
            "No Payment Method:",
            "Premium:",
        };
    }

    public List<BorderLineWidth> GetPaymentBorders()
    {
        return new List<BorderLineWidth>
        {
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.DescriptionCash),
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.DescriptionCheque),
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.DescriptionNoPayment),
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.DescriptionPremium),
        };
    }

    public List<BorderLineWidth> GetCleanerTagLineBorders()
    {
        return new List<BorderLineWidth>
        {
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.CleanersTag),
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.CleanersName),
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.CleanersHours),
        };
    }

    public List<BorderLineWidth> GetCleanerLineBorders()
    {
        return new List<BorderLineWidth>
        {
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.CleanersName),
            BorderLineWidthCollection.GetBorderLineWidth(BorderLineType.CleanersHours),
        };
    }

    #endregion

    #region Creation

    public void AddTextWithLine(string content, int borderLineStart = -1, int borderLineEnd = -1)
    {
        AddText(content);
        DrawBorderLine(borderLineStart, borderLineEnd);
    }

    public void AddText(string content, int xSpacing = -1)
    {
        if (xSpacing < 0)
        {
            xSpacing = DC.XMargin;
        }

        this.currentPDFPage.Page.addText(content, xSpacing, this.currentPDFPage.CurrentTextLineY, DC.Active.Font, DC.Active.FontSize);
    }

    public void DrawBorderLine(int lineStart = -1, int lineEnd = -1)
    {
        if (lineEnd < 0)
        {
            lineEnd = this.currentPDFPage.Width - DC.XMargin;
        }

        if (lineStart < 0)
        {
            lineStart = DC.XMargin;
        }

        this.currentPDFPage.Page.drawLine(lineStart, this.currentPDFPage.CurrentBorderLineY, lineEnd, this.currentPDFPage.CurrentBorderLineY, DC.Active.LineStyle, DC.Active.BorderLinePDFColor, DC.Active.BorderLineWidth);
    }

    public void DrawLine(int x1, int y1, int x2, int y2)
    {
        this.currentPDFPage.Page.drawLine(x1, y1, x2, y2);
    }

    public void DrawX(int x1, int y1, int x2, int y2)
    {
        DrawLine(x1, y1, x2, y2);
        DrawLine(x1, y2, x2, y1);
    }

    public void AddMultiText(List<string> content, bool addLine = false, int checkedBoxIndex = -1, int lineStart = -1, int lineEnd = -1)
    {
        int contentSpace = this.currentPDFPage.Width / content.Count;
        int currentContentIndex = 0;
        int currentTextX;

        foreach (string entry in content)
        {
            currentTextX = (currentContentIndex * contentSpace) + DC.XMargin;
            AddText(entry, currentTextX);
            currentContentIndex++;

        }

        if (addLine)
        {
            DrawBorderLine(lineStart, lineEnd);
        }
    }

    public void AddMultiText(List<string> content, List<BorderLineWidth> lineWidths, bool addCheckBox = false, int checkedBoxIndex = -1)
    {
        int contentSpace = this.currentPDFPage.Width / content.Count;
        int currentContentIndex = 0;
        int currentTextX;

        foreach (string entry in content)
        {
            currentTextX = (currentContentIndex * contentSpace) + DC.XMargin;
            AddText(entry, currentTextX);
            currentContentIndex++;
        }

        currentContentIndex = 0;

        foreach (BorderLineWidth borderLine in lineWidths)
        {
            int checkBoxX1;
            int checkBoxY1;
            int checkBoxX2;
            int checkBoxY2;

            DrawBorderLine(borderLine.XStart, borderLine.XEnd);

            if (addCheckBox)
            {
                checkBoxX1 = borderLine.XEnd + 5;
                checkBoxY1 = this.currentPDFPage.CurrentBorderLineY;
                checkBoxX2 = borderLine.XEnd + 15;
                checkBoxY2 = this.currentPDFPage.CurrentBorderLineY + 10;

                DrawRectangle(checkBoxX1, checkBoxY1, checkBoxX2, checkBoxY2);

                if (checkedBoxIndex == currentContentIndex)
                {
                    DrawX(checkBoxX1, checkBoxY1, checkBoxX2, checkBoxY2);
                }
            }

            currentContentIndex++;
        }
    }

    public void AddParagraph(string content)
    {
        this.currentPDFPage.Page.addParagraph(
            content,
            DC.XMargin,
            this.currentPDFPage.CurrentTextLineY,
            DC.Active.Font,
            DC.Active.FontSize,
            DC.Active.FontSize,
            this.currentPDFPage.Width - (DC.XMargin * 2)
            );
    }

    public void CreatePageLine(int y)
    {
        this.currentPDFPage.Page.drawLine(DC.XMargin, 770, 562, 770, predefinedLineStyle.csNormal, new pdfColor(0, 0, 0), 1);
    }

    public void DrawJobDescriptionContainer()
    {
        DrawRectangle(DC.XMargin / 2, this.currentPDFPage.CurrentBorderLineY, this.currentPDFPage.Width - (DC.XMargin / 2), DC.YMargin);
    }

    public void DrawRectangle(int x1, int y1, int x2, int y2)
    {
        this.currentPDFPage.Page.drawRectangle(x1, y1, x2, y2, DC.Active.RectanglePDFColor, new pdfColor(255, 255, 255));
    }

    public void CreateContent(string content)
    {

    }

    private void CreateContentContainer(pdfPage page)
    {

    }

    private void PopulateContent()
    {

    }

    #endregion
}
