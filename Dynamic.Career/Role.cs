using Dynamic.Runtime;

namespace Dynamic.Career;

public class Role(Company company) : ActiveObject<Role>
{
    public Company Company { get; set; } = company;

    public List<Recommendation> Recommendations { get; set; } = []; 

    public List<Skill> Skills { get; set; } = [];
    
    public int? StartYear { get; set; }

    public int? EndYear { get; set; } 
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
