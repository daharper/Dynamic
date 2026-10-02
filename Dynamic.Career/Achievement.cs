using Dynamic.Runtime;

namespace Dynamic.Career;

public class Achievement(string name, string organisation = "") : ActiveData<Achievement>(name)
{
    public string Organisation { get; set; } = organisation;
}