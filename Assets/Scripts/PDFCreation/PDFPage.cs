using sharpPDF;
using sharpPDF.Fonts;
using sharpPDF.Enumerators;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using DC = DocumentCreator;

public class PDFPage
{
    private pdfPage page;
    public pdfPage Page => page;

    private int currentTextLineY;
    public int CurrentTextLineY => currentTextLineY;
    private int currentBorderLineY;
    public int CurrentBorderLineY => currentBorderLineY;
    private int lineSpace = 10;
    public int Width => page.width;
    public int Height => page.height;

    public PDFPage(pdfDocument document)
    {
        this.page = document.addPage();
    }

    #region Getters

    #endregion

    #region Setters

    public void SetNewPageCoordinates()
    {
        this.currentTextLineY = this.page.height - DC.YMargin;
        SetBorderLineY();
    }

    public void SetNewTextLineY(int extraSpace = 0)
    {
        this.currentTextLineY -= this.lineSpace + DC.Active.FontSize + DC.Active.BorderLineWidth + extraSpace;
    }

    public void SetBorderLineY()
    {
        try
        {
            this.currentBorderLineY = this.currentTextLineY - DC.Active.BorderLineWidth - 1;
        }
        catch (Exception e)
        {
            LogHelper.Active.LogError(e.Message);

            this.currentBorderLineY = 0;
        }
    }

    #endregion

    #region Actions

    public void OnTextLineAdded(int extraLineSpace = 0)
    {
        SetNewTextLineY(extraLineSpace);
        SetBorderLineY();
    }

    #endregion

    #region Creation/Population



    #endregion
}
