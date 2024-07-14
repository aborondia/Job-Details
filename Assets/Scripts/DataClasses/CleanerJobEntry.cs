public class CleanerJobEntry
{
    private string name;
    public string Name => this.name;
    private string cleanerObjectId;
    public string CleanerObjectId => this.cleanerObjectId;
    private float hoursWorked;
    public float HoursWorked => this.hoursWorked;

    public CleanerJobEntry() { }

    public CleanerJobEntry(string cleanerObjectId, string name, float hoursWorked)
    {
        this.cleanerObjectId = cleanerObjectId;
        this.name = name;
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

    public void SetCleanerObjectId(string value)
    {
        this.cleanerObjectId = value;
    }
}
