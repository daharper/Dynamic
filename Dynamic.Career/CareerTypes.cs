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

public sealed record Skill
{
    public string Name { get; set; } = "";

    public Skill(string name, Skill? parent = null)
    {
        Name = name;
        Parent = parent;
    }

    public Skill? Parent { get; set; }

    public List<Skill> Children { get; } = [];

    public List<Skill> Categories { get; } = [];

    public bool HasParent => Parent is not null;

    public bool HasChild => Children.Count > 0;

    public bool HasCategories => Categories.Count > 0;

    public Skill With(params Skill[] skills)
    {
        foreach (var skill in skills)
        {
            if (skill.HasParent)
            {
                if (skill.Categories.All(c => c.Name != Name))
                    skill.Categories.Add(this);
            }
            else
            {
                skill.Parent = this;
            }

            if (Children.All(c => c.Name != skill.Name))
                Children.Add(skill);
        }

        return this;
    }
}

//public sealed record Skill
//{
//    public string Name { get; set; } = "";

//    public Skill(string name, Skill? parent = null)
//    {
//        Name = name;

//        if (parent is not null)
//            Parents.Add(parent);
//    }

//    public List<Skill> Parents { get; } = [];

//    public List<Skill> Children { get; } = [];

//    public bool HasParent => Parents.Count > 0;

//    public bool HasChild => Children.Count > 0;

//    public Skill With(params Skill[] skills)
//    {
//        foreach (var skill in skills)
//        {
//            // They don't have us as a parent
//            if (skill.Parents.All(p => p.Name != Name))
//            {
//                skill.Parents.Add(this);
//            }

//            // We don't have them as a child
//            if (Children.All(c => c.Name != skill.Name))
//            {
//                Children.Add(skill);
//            }
//        }

//        return this;
//    }
//}

public sealed class SkillComparer : IEqualityComparer<Skill>
{
    public bool Equals(Skill? x, Skill? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null || y is null) return false;

        return StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);
    }

    public int GetHashCode(Skill obj)
        => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
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