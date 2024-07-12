public class CleanerJobEntry
{
    private string name;
    public string Name => this.name;
    private float hoursWorked;
    public float HoursWorked => this.hoursWorked;

    public CleanerJobEntry() { }

    public CleanerJobEntry(string name, float hoursWorked)
    {
        this.name = name;
        this.hoursWorked = hoursWorked;
    }
}
