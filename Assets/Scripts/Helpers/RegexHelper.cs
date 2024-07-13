using System.Text.RegularExpressions;

public static class RegexHelper
{
    private static Regex numberRegex = new Regex("^[0-9]*$");
    public static Regex NumberRegex => numberRegex;
    private static Regex floatRegex = new Regex(@"^(?:\d+|\d*\.\d{1,2})$");
    public static Regex FloatRegex => floatRegex;
    private static Regex timeRegex = new Regex("^(1[0-2]|0?[1-9]):[0-5][0-9]");
    public static Regex TimeRegex => timeRegex;
    private static Regex hourRegex = new Regex("^(1[0-2]|[1-9])$");
    // private static Regex hourRegex = new Regex("^[0-9][1-2]{0,1}$");
    public static Regex HourRegex => hourRegex;
    private static Regex minuteRegex = new Regex("^([0-5]?[0-9])$");
    // private static Regex minuteRegex = new Regex("^([0-5]?[0-9]|59)$");
    public static Regex MinuteRegex => minuteRegex;
}
