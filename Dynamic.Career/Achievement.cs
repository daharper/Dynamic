using Dynamic.Runtime;

namespace Dynamic.Career;

public class Achievement(string name, string organisation = "") : ActiveObject<Achievement>
{
    public string Name { get; set; } = name;
    
    public string Organisation { get; set; } = organisation;
}