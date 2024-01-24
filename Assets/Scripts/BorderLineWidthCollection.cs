using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BorderLineWidthCollection
{
    public enum BorderLineTypeEnum
    {
        CleanersHours,
        CleanersName,
        CleanersTag,
        DescriptionCash,
        DescriptionCheque,
        DescriptionNoPayment,
        DescriptionPremium,
    }
    private static BorderLineWidth[] borderLineWidths =
    {
        new BorderLineWidth(485, DocumentCreator.DefaultPageWidth - DocumentCreator.XMargin),
        new BorderLineWidth(265, 455),
        new BorderLineWidth(DocumentCreator.XMargin, 100),
        new BorderLineWidth(DocumentCreator.XMargin, 80),
        new BorderLineWidth(204, 245),
        new BorderLineWidth(355, 475),
        new BorderLineWidth(508, DocumentCreator.DefaultPageWidth - DocumentCreator.XMargin)
    };

    public static BorderLineWidth GetBorderLineWidth(BorderLineTypeEnum borderLineTypeEnum)
    {
        if ((int)borderLineTypeEnum < borderLineWidths.Length)
        {
            return borderLineWidths[(int)borderLineTypeEnum];
        }
        else
        {
            return null;
        }
    }
}
