using Dynamic.Runtime;

namespace Dynamic.Career;

public class Role : ActiveObject<Role>
{
    public Role(Company company)
    {
        Company = company;
    }

    public Company Company { get; set; }

    public string StartYear { get; set; }

    public string EndYear { get; set; }
}

public sealed class RoleComparer : IEqualityComparer<Role>
{
    public bool Equals(Role? x, Role? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null || y is null) return false;

        return StringComparer.OrdinalIgnoreCase.Equals(x.Company.Name, y.Company.Name);
    }

    public int GetHashCode(Role obj)
        => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Company.Name);
}
