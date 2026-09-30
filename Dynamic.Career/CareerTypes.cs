using Dynamic.Runtime;

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

public enum CompanyScale
{
    NotSpecified,
    Startup,
    Scaleup,
    Small,
    Medium,
    Enterprise
}

public readonly record struct Money(decimal Amount, string Currency);

public sealed record Achievement(string Name, string Organisation = "")
{
    public string Organisation { get; set; } = Organisation;
}

public sealed record Highlight(Company Company, string Description = "")
{
    public string Description { get; set; } = Description;
}

public sealed record Preference(string Country, Option Options = Option.Remote)
{
    public Option Options { get; set; } = Options;
}

public sealed record Company(string Name, string Description = "", CompanyScale Scale = CompanyScale.NotSpecified)
{
    public string Description { get; set; } = Description;

    public CompanyScale Scale { get; set; } = Scale;
}

public sealed class CompanyComparer : IEqualityComparer<Company>
{
    public bool Equals(Company? x, Company? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null || y is null) return false;

        return StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);
    }

    public int GetHashCode(Company obj)
        => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
}