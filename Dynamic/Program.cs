using Dynamic.Career;
using Dynamic.Runtime;
using System.Text;
using Dynamic.Application;
using Dynamic.Application.Scribe;

Console.OutputEncoding = Encoding.UTF8;

ActiveRuntime.Register(typeof(Resume));

ByCode();
//ByScript();

return;

static void ByCode()
{
    var r = ResumeFactory.Create();

    ResumeWriter.Write(r, ResumeSection.All);
}

static void ByScript()
{
    var r = new Resume();

    var text = File.ReadAllText("resume.txt");

    Eval.Run(text, () => r);

    ResumeWriter.Write(r, ResumeSection.All);
}
