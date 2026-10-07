using Dynamic.Runtime;

namespace Dynamic.Career;

public class Recommendation(Company company, string name = "", string title = "") : ActiveObject<Recommendation>
{
    public Company Company { get; set; } = company;
    
    public string Name { get; set; } = name;
    
    public string Description { get; set; } = "";

    public string Title { get; set; } = title;

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