using Dynamic.Runtime;

namespace Dynamic.Career;

public sealed class Resume : ActiveObject<Resume>
{
    public Resume()
    {
        Highlights.Factory = (Func<string, Highlight>)(n => new Highlight(Companies.GetOrAdd(new Company(n))));
    }

    public string Name { get; set; } = "";
    
    public string Location { get; set; } = "";
    
    public Availability Availability { get; set; }

    public Money MinimumSalary { get; set; }

    public dynamic Achievements { get; } = new ActiveList<Achievement>(n => new Achievement(n));

    public dynamic Eligible { get; } = new ActiveList<Preference>(n => new Preference(n));

    public dynamic Desirable { get; } = new ActiveList<Preference>(n => new Preference(n));

    public dynamic Companies { get; } = new ActiveList<Company>(n => new Company(n), new CompanyComparer());

    public dynamic Highlights { get; } = new ActiveList<Highlight>();

    public dynamic Skills { get; } = new ActiveList<Skill>(n => new Skill(n), new SkillComparer());

    public void AddSkill(Skill skill, params Skill[] childSkills)
    {
        skill = Skills.GetOrAdd(skill);

        foreach (var childSkill in childSkills)
        {
            var sk = Skills.GetOrAdd(childSkill);
            sk.Parent = skill;
        }
    }

    public void Show()
    {
        var output = $"""
                      Name: {Name}
                      Option: {Location}
                      Availability: {Availability}
                      Minimum Salary: {MinimumSalary}
                      
                      """;

        Console.WriteLine(output);

        Console.WriteLine("Eligible Preferences:");

        foreach (var e in Eligible)
        {
            Console.WriteLine($"  • {e.Country} ({e.Options})");
        }

        Console.WriteLine("Desirable Preferences:");

        foreach (var d in Desirable)
        {
            Console.WriteLine($"  • {d.Country} ({d.Options})");
        }

        Console.WriteLine("Companies:");

        foreach (var c in Companies)
        {
            Console.WriteLine($"  • {c.Name}, {c.Description} ({c.Scale})");
        }

        Console.WriteLine("Achievements:");

        foreach (var a in Achievements)
        {
            Console.WriteLine($"  • {a.Name} ({a.Organisation})");
        }

        Console.WriteLine("Highlights:");

        foreach (var h in Highlights)
        {
            Console.WriteLine($"  • {h.Company.Name}: {h.Description}");
        }

        Console.WriteLine("Skills:");

        foreach (var s in Skills)
        {
            Console.WriteLine($"  • {s.Name}");

            if (s.HasParent)
            {
                Console.WriteLine("        [parent]");
                Console.WriteLine($"        • {s.Parent.Name}");
            }

            if (s.HasChild)
            {
                Console.WriteLine("        [children]");

                foreach (var child in s.Children)
                {
                    Console.WriteLine($"        • {child.Name}");
                }
            }

            if (s.HasParent)
            {
                Console.WriteLine("        [categories]");

                foreach (var category in s.Categories)
                {
                    Console.WriteLine($"        • {category.Name}");
                }
            }
        }
    }
}