public class CleanerJobEntry
{
    private string name;
    public string Name => this.name;
    private float hoursWorked;
    public float HoursWorked => this.hoursWorked;

    public CleanerJobEntry() { }

    public CleanerJobEntry(string cleanerName)
    {
        this.name = cleanerName;
        this.hoursWorked = 1;
    }

    public CleanerJobEntry(string cleanerName, float hoursWorked)
    {
        this.name = cleanerName;
        this.hoursWorked = hoursWorked;
    }

    public void SetName(string value)
    {
        this.name = value;
    }

    public void SetHoursWorked(float value)
    {
        this.hoursWorked = value;
    }
}
