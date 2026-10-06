using Dynamic.Runtime;

namespace Dynamic.Career;

public class Recommendation : ActiveObject<Recommendation>
{
    public Recommendation(Company company, string name = "", string title = "") : base(name)
    {
        Company = company;
        Title = title;
    }
    
    public Company Company { get; set; }
    
    public string LinkedIn { get; set; } = "";
}

public sealed class RecommendationComparer : IEqualityComparer<Recommendation>
{
    public bool Equals(Recommendation? x, Recommendation? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null || y is null) return false;

        return StringComparer.OrdinalIgnoreCase.Equals(x.Company.Name, y.Company.Name);
    }

    public int GetHashCode(Recommendation obj)
        => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Company.Name);
}