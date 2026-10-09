using Dynamic.Career;
using Dynamic.Runtime;
using System.Text;
using Dynamic.Application;

Console.OutputEncoding = Encoding.UTF8;

ActiveRuntime.Register(typeof(Resume));

ByCode();
//ByScript(); - resume.txt needs updating to new semantics

return;

static void ByCode()
{
    const ResumeSection sections =
        ResumeSection.Detail |
        ResumeSection.Preference | 
        ResumeSection.Achievement |
        ResumeSection.Company |
        ResumeSection.Highlight |
        ResumeSection.Recommendation |
        ResumeSection.Skill | 
        ResumeSection.Role;
    
    var r = ResumeFactory.Create(sections);

    ResumeWriter.Write(r, ResumeSection.Role);
}

static void ByScript()
{
    var r = new Resume();

    var text = File.ReadAllText("resume.txt");

    Eval.Run(text, () => r);

    ResumeWriter.Write(r, ResumeSection.All);
}
