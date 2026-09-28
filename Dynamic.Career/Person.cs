using Dynamic.Runtime;

namespace Dynamic.Career;

public sealed class Person : ActiveObject<Person>
{
    public string Name { get; set; } = "";
    
    public string Location { get; set; } = "";
    
    public Availability Availability { get; set; }
    
    public WorkingArrangement WorkingArrangements { get; set; }

    public Money MinimumSalary { get; set; }

    public dynamic Achievements { get; } = new ActiveList<Achievement>(n => new Achievement(n));

    public dynamic RightToWork { get; } = new ActiveList<string>();

    public void Show()
    {
        var output = $"""
                      Name: {Name}
                      Location: {Location}
                      Availability: {Availability}
                      Working Arrangements: {WorkingArrangements}
                      Minimum Salary: {MinimumSalary}
                      
                      """;

        Console.WriteLine(output);

        Console.WriteLine("Rights to Work:");

        foreach (var right in RightToWork)
        {
            Console.WriteLine($"  - {right}");
        }

        Console.WriteLine("Achievements:");

        foreach (var achievement in Achievements)
        {
            Console.WriteLine($"  - {achievement.Name} ({achievement.Organisation})");
        }
    }
}