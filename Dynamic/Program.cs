using Dynamic.Career;
using Dynamic.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using Dynamic.Application;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

Console.OutputEncoding = Encoding.UTF8;

ActiveRuntime.Register(typeof(Resume));

ByCode();
//ByScript();

return;

static void ByCode()
{
    var r = ResumeFactory.Create();

    //r.Achievements.Mvp("Embarcadero");

    //var skills = r.Skills;
    //var technologies = skills.Technologies;
    //var languages = skills.Languages;

    //var delphi = skills.Delphi
    //    .Let("Versions", new List<string> { "1", "2", "3", "4", "5", "6", "7", "10", "XE", "11", "12", "13" })
    //    .Note("First used Delphi in 1998")
    //    .Includes(skills.ObjectPascal, skills.RTL, skills.VCL);

    //var dotNet = skills.DotNet
    //    .Alias(".NET")
    //    .Includes(skills.CSharp, skills.DotNetCore.Alias(".NET Core"));

    //skills.CSharp
    //    .Alias("C#")
    //    .Let("Versions", Enumerable.Range(1, 15).Select(i => i.ToString()).ToList())
    //    .Note("First used C# in 2001 whilst in beta");

    //skills.ObjectPascal.Alias("OP");

    //languages.Generalizes(skills.CSharp, skills.ObjectPascal);

    //technologies.Includes(languages, skills.CSharp, skills.ObjectPascal, dotNet, delphi);

    r.Show();
}

static void ByScript()
{
    var r = new Resume();

    var text = File.ReadAllText("resume.txt");

    Eval.Run(text, () => r);

    r.Show();
}
