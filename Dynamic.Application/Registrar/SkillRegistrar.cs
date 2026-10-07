using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class SkillRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Skill;

    public void Register(Resume r)
    {
        Skill = r.Skills;
        
        Technology = Skill.Technology;
        Language = Skill.Language;
        Framework = Skill.Framework;
        
        DefineSkills(r);
        ClassifySkills(r);
    }
    
    private dynamic Skill { get; set; }= null!;
    private dynamic Technology { get; set; } = null!;
    private dynamic Language { get; set; } = null!;
    private dynamic Framework { get; set; } = null!;
    
    private void DefineSkills(Resume r)
    {
        Skill.CSharp
            .ListedIn(Language)
            .Title("C#")
            .Note("First used C# in 2001 whilst in beta, and have used it across all versions ever since.");

        Skill.ObjectPascal
            .ListedIn(Language)
            .Aka("OP")
            .Title("Object Pascal");

        Skill.Delphi.Note("First used C# in 1998");

        Skill.HTML.Aka("HTML5");

        Skill.JavaScript.Aka = "ECMAScript";
        
        Skill.CSS.Aka("Cascading Style Sheets");

        Skill.COM
            .ListedIn(Framework);

    }
    
    private void ClassifySkills(Resume r)
    {
    }
}