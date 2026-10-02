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

public class Company : ActiveData<Skill>
{
    public Company(string name, string description = "", CompanyScale scale = CompanyScale.NotSpecified) : base(name)
    {
        Scale = scale;
        Description = description;
    }

    public CompanyScale Scale { get; set; }
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