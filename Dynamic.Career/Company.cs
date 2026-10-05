using Dynamic.Runtime;

namespace Dynamic.Career;

public enum CompanyScale
{
    NotSpecified,
    None,
    Startup,
    Scaleup,
    Small,
    Medium,
    Enterprise
}

public enum Country
{
    NotSpecified,
    Australia,
    Singapore,
    UnitedKingdom
}

public class Company : ActiveObject<Company>
{
    public Company(string name, string description = "") 
        : base(name)
    {
        Description = description;
    }

    public CompanyScale Scale { get; set; } = CompanyScale.NotSpecified;

    public Country Country { get; set; } = Country.NotSpecified;
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