using Dynamic.Runtime;

namespace Dynamic.Career;

public sealed class Resume : ActiveObject<Resume>
{
    public Resume()
    {
        Highlights.Factory = (Func<string, Highlight>)(n => new Highlight(Companies.GetOrAdd(new Company(n))));
        Roles.Factory = (Func<string, Role>)(n => new Role(Companies.GetOrAdd(new Company(n))));
    }

    public string Email { get; set; } = "";

    public string LinkedIn { get; set; } = "";

    public string Location { get; set; } = "";
    
    public Availability Availability { get; set; }

    public Money MinimumSalary { get; set; }

    public dynamic Roles { get; } = new ActiveList<Role>(null, new RoleComparer());

    public dynamic Achievements { get; } = new ActiveList<Achievement>(n => new Achievement(n));

    public dynamic Eligible { get; } = new ActiveList<WorkPreference>(n => new WorkPreference(n));

    public dynamic Desirable { get; } = new ActiveList<WorkPreference>(n => new WorkPreference(n));

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
                      Title: {Title}
                      Email: {Email}
                      LinkedIn: {LinkedIn}
                      Location: {Location}
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

        Console.WriteLine("Achievements:");

        foreach (var a in Achievements)
        {
            Console.WriteLine($"  • {a.Name} ({a.Organisation})");
        }

        Console.WriteLine("Highlights:");

        foreach (var h in Highlights)
        {
            Console.WriteLine($"  • {h.Company.Name}: {h.Description}");

            if (h.HasLink)
            {
                foreach (var link in h.Links)
                {
                    Console.WriteLine($"    - {link.Title} ({link.Url})");
                }
            }
        }

        Console.WriteLine("Companies:");

        foreach (var c in Companies)
        {
            Console.WriteLine($"{c.Title} ({c.Scale})");
            Console.WriteLine(c.Description);
            Console.WriteLine();

            if (c.HasNote)
            {
                Console.WriteLine("[notes]");

                foreach (var note in c.Notes)
                {
                    Console.WriteLine(note);
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine();

        Console.WriteLine("Skills:");

        foreach (var s in Skills)
        {
            var aliases = s.HasAlias ? "(" + string.Join(" ", s.Aliases) + ")" : "";

            Console.WriteLine($"  • {s.Name} {aliases}");

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

            if (s.HasMember)
            {
                Console.WriteLine("        [members]");

                foreach (var member in s.AllMembers)
                {
                    Console.WriteLine($"        • {member.Name}");
                }
            }


            if (s.IsMember)
            {
                Console.WriteLine("        [member of]");

                foreach (var memberOf in s.MemberOf)
                {
                    Console.WriteLine($"        • {memberOf.Name}");
                }
            }

            if (s.HasProp)
            {
                Console.WriteLine("        [properties]");

                foreach (var prop in s.Props)
                {
                    Console.WriteLine($"        • {prop.Key} = {s.AsStr(prop.Key)}");
                }
            }

            if (s.HasNote)
            {
                Console.WriteLine("        [notes]");

                foreach (var note in s.Notes)
                {
                    Console.WriteLine($"        • {note}");
                }
            }
        }

        Console.WriteLine("Roles:");

        foreach (var r in Roles)
        {
            Console.WriteLine($"  • {r.StartYear} - {r.EndYear} {r.Company.Name} {r.Title}");
            Console.WriteLine($"    {r.Description}");
        }
    }
}