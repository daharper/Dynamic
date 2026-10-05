using Dynamic.Runtime;

namespace Dynamic.Career;

[Flags]
public enum Option
{
    None = 0,
    Remote = 1,
    Onsite = 2,
    Hybrid = 4,
    OnsiteOnRelocation = 8,
    HybridOnRelocation = 16,
    AnyOnRelocation = 32
}

public class Preference(string country, Option options = Option.Remote) : ActiveObject<Preference>(country)
{
    public string Country { get; set; } = country;

    public Option Options { get; set; } = options;
}