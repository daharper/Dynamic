using Dynamic.Runtime;

namespace Dynamic.Career;

public sealed class Resume : ActiveObject<Resume>
{
    public Resume()
    {
        Highlights.Factory = 
            (Func<string, Highlight>)
            (n => new Highlight(Companies.GetOrAdd(new Company(n))));
        
        Roles.Factory = 
            (Func<string, Role>)
            (n => new Role(Companies.GetOrAdd(new Company(n))));
        
        Recommendations.Factory = 
            (Func<string, Recommendation>)
            (n => new Recommendation(Companies.GetOrAdd(new Company(n))));
    }

    public string Email { get; set; } = "";

    public string LinkedIn { get; set; } = "";
    
    public string GitHub { get; set; } = "";

    public string Location { get; set; } = "";
    
    public Availability Availability { get; set; }

    public Money MinimumSalary { get; set; }

    public dynamic Roles { get; } = new ActiveList<Role>(null, new RoleComparer());

    public dynamic Achievements { get; } = new ActiveList<Achievement>(n => new Achievement(n));

    public dynamic Eligible { get; } = new ActiveList<Preference>(n => new Preference(n));

    public dynamic Desirable { get; } = new ActiveList<Preference>(n => new Preference(n));

    public dynamic Companies { get; } = new ActiveList<Company>(n => new Company(n), new CompanyComparer());

    public dynamic Highlights { get; } = new ActiveList<Highlight>();

    public dynamic Skills { get; } = new ActiveList<Skill>(n => new Skill(n), new SkillComparer());

    public dynamic Recommendations { get; } = new ActiveList<Recommendation>(null, new RecommendationComparer());
    
    public void AddSkill(Skill skill, params Skill[] childSkills)
    {
        skill = Skills.GetOrAdd(skill);

        foreach (var childSkill in childSkills)
        {
            var sk = Skills.GetOrAdd(childSkill);
            sk.Parent = skill;
        }
    }
}