public class TimeContentHolder
{
    public int? Hour { get; set; }
    public int? Minutes { get; set; }

    public TimeContentHolder() { }

    public TimeContentHolder(int hour, int minutes)
    {
        this.Hour = hour;
        this.Minutes = minutes;
    }
}
