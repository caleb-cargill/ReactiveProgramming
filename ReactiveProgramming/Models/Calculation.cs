namespace ReactiveProgramming.Models;

public class Calculation
{
    public string Name { get; set; }
    public string Body { get; set; }
    public string Updates { get; set; }
    public string UpdatesSubject { get; set; }
    public List<(string Subject, string Member)> MemberArguments { get; set; }
}
