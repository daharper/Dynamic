using System.Text;
using Dynamic.Application.Scribe;
using Dynamic.Career;

namespace Dynamic.Application;

public static class ResumeWriter
{
    private static readonly List<IResumeScribe> Scribes =
    [
        new CompanyScribe()
    ];
    
    public static void Write(Resume r, ResumeSection sections)
    {
        var sb = new StringBuilder();
        
        foreach (var scribe in Scribes)
        {
            if ((sections & scribe.Section) == scribe.Section)
            {
                scribe.Write(r, sb);
            }
        }

        Console.WriteLine(sb.ToString());
    }
}