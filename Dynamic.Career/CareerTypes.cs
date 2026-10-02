namespace Dynamic.Career;

public enum Availability
{
    Immediately,
    OneWeek,
    TwoWeeks,
    OneMonth,
    TwoMonths,
    ThreeMonths,
}

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

public readonly record struct Money(decimal Amount, string Currency);

public sealed record Achievement(string Name, string Organisation = "")
{
    public string Organisation { get; set; } = Organisation;
}

public sealed record Preference(string Country, Option Options = Option.Remote)
{
    public Option Options { get; set; } = Options;
}

