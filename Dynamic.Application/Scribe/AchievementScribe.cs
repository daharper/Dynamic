using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class AchievementScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Achievement;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Achievements:");

        foreach (var a in r.Achievements)
        {
            sb.AppendLine($"  • {a.Name} ({a.Organisation})");
        }
    }
}