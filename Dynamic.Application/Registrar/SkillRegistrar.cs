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
        Windows = Skill.Windows;
        Linux = Skill.Linux;
        Web = Skill.Web;
        Component = Skill.Component;
        Desktop = Skill.Desktop;
        Database = Skill.Database;
        Report = Skill.Report.Aka("Reports", "Reporting");
        Delphi = Skill.Delphi.Note("First used Delphi in 1998");
        
        DefineSkills(r);
        ClassifySkills(r);
    }
    
    private dynamic Skill { get; set; } = null!;
    private dynamic Technology { get; set; } = null!;
    private dynamic Language { get; set; } = null!;
    private dynamic Framework { get; set; } = null!;
    private dynamic Windows { get; set; } = null!;
    private dynamic Web { get; set; } = null!;
    private dynamic Linux { get; set; } = null!;
    private dynamic Component { get; set; } = null!;
    private dynamic Desktop { get; set; } = null!;
    private dynamic Database { get; set; } = null!;
    private dynamic Report { get; set; } = null!;
    private dynamic Delphi { get; set; } = null!;
    
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

        Skill.VCL
            .Title("Visual Component Library")
            .ListedIn(Delphi, Desktop, Framework);
        
        Skill.HTML.Aka("HTML5").ListedIn(Web);

        Skill.JavaScript.Aka("ECMAScript").ListedIn(Web);
        
        Skill.CSS.Aka("Cascading Style Sheets").ListedIn(Web);

        Skill.COM.ListedIn(Windows, Framework);

        Skill.Win32.ListedIn(Windows, Framework);

        Skill.DevExpress.ListedIn(Delphi, Component);
        
        Skill.SQL.ListedIn(Database);
        
        Skill.BDE.Aka("Borland Database Engine").ListedIn(Delphi, Database);
        
        Skill.MySQL.ListedIn(Database);
        
        Skill.CrystalReports.ListedIn(Report);
    }
    
    private void ClassifySkills(Resume r)
    {
    }
}