using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class SkillRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Skill;

    public void Register(Resume r)
    {
        DefineCoreSkills(r);
        
        DefineDelphiSkills();
        DefineWebSkills();
        DefinePlatformSkills();
        DefineDatabaseSkills();
        DefineReportSkills();
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
    
    private void DefineCoreSkills(Resume r)
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
    }

    private void DefineDelphiSkills()
    {
        Skill.ObjectPascal
            .ListedIn(Language)
            .Aka("OP")
            .Title("Object Pascal");

        Skill.VCL
            .Title("Visual Component Library")
            .ListedIn(Delphi, Desktop, Framework);

        Skill.DevExpress.ListedIn(Delphi, Component);
    }

    private void DefineWebSkills()
    {
        Skill.HTML.Aka("HTML5").ListedIn(Web);
        Skill.JavaScript.Aka("ECMAScript").ListedIn(Web);
        Skill.CSS.Aka("Cascading Style Sheets").ListedIn(Web);
    }
    
    private void DefineDatabaseSkills()
    {
        Skill.BDE.Aka("Borland Database Engine").ListedIn(Delphi, Database);
        Skill.SQL.ListedIn(Database);
        Skill.MySQL.ListedIn(Database);
    }
    
    private void DefinePlatformSkills()
    {
        Skill.COM.ListedIn(Windows, Framework);
        Skill.Win32.ListedIn(Windows, Framework);
    }
    
    private void DefineReportSkills()
    {
        Skill.CrystalReports.ListedIn(Report);
    }
}