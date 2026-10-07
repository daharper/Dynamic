using Dynamic.Runtime;

namespace Dynamic.Career;

public sealed class Skill(string name) : ActiveObject<Skill>
{
    public string Name { get; set; } = name;
}

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
