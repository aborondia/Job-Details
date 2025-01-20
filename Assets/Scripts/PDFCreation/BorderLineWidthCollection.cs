
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
        new BorderLineWidth(390, 470),
        new BorderLineWidth(220, 385),
        new BorderLineWidth(DocumentCreator.XMargin, 110),
        new BorderLineWidth(DocumentCreator.XMargin, 85),
        new BorderLineWidth(178, 230),
        new BorderLineWidth(305, 343),
        new BorderLineWidth(433, 493)
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
