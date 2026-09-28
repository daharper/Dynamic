using Dynamic.Runtime;

namespace Dynamic.Career;

public sealed class Person : ActiveObject<Person>
{
    public string FirstName { get; set; } = "";

    public int Years { get; set; }
}