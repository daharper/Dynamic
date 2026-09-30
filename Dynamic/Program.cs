using System.Text;
using Dynamic.Runtime;
using Dynamic.Career;

Console.OutputEncoding = Encoding.UTF8;

ActiveRuntime.Register(typeof(Resume));

ByCode();
//ByScript();

return;

static void ByCode()
{
    var r = new Resume
    {
        Name = "David Harper",
        Location = "Spennymoor, County Durham, United Kingdom",
        Availability = Availability.Immediately,
        MinimumSalary = new Money(50_000m, "GBP")
    };

    r.Eligible.Australia.Options = Option.Remote | Option.AnyOnRelocation;
    r.Eligible.Ireland.Options = Option.Remote;
    r.Eligible.UnitedKingdom.Options = Option.Remote;
    r.Eligible.NorthEastUnitedKingdom.Options = Option.Remote | Option.Hybrid;

    r.Desirable.Singapore.Options = Option.Remote | Option.AnyOnRelocation;
    r.Desirable.USA.Options = Option.Remote | Option.AnyOnRelocation;

    r.Achievements.Mvp("Embarcadero");

    r.Companies.HeuLabs("HeuLabs was a fast-growing, award-winning, innovative Singaporean EdTech startup", CompanyScale.Startup);
    r.Companies.STEngineering("Global technology engineering group with customers in over 100 countries.", CompanyScale.Enterprise);

    r.Highlights.HeuLabs.Description = "Presented HeuCampus live at the Microsoft Singapore launch event for Visual Studio 2005";

    r.Skills.Technologies.With(
        r.Skills.Languages.With(
            r.Skills.CSharp,
            r.Skills.ObjectPascal),
        r.Skills.DotNet.With(
            r.Skills.CSharp,
            r.Skills.DotNetCore),
        r.Skills.Delphi.With(
            r.Skills.ObjectPascal,
            r.Skills.RTL,
            r.Skills.VCL));

    r.Show();
}

static void ByScript()
{
    var r = new Resume();

    var text = File.ReadAllText("resume.txt");

    Eval.Run(text, () => r);

    r.Show();
}