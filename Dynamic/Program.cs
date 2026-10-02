using Dynamic.Career;
using Dynamic.Runtime;
using System.Text;
using Dynamic.Application;

Console.OutputEncoding = Encoding.UTF8;

ActiveRuntime.Register(typeof(Resume));

ByCode();
//ByScript();

return;

static void ByCode()
{
    var r = ResumeFactory.Create();

    // todo: improve the semantics
    Console.WriteLine($"{r.Roles.HeuLabs.Props["Rating"]}");

    r.Show();
}

static void ByScript()
{
    var r = new Resume();

    var text = File.ReadAllText("resume.txt");

    Eval.Run(text, () => r);

    r.Show();
}
