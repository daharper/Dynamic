using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class PreferenceScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Preference;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Eligible Preferences:");

        foreach (var e in r.Eligible)
        {
            sb.AppendLine($"  • {e.Country} ({e.Options})");
        }

        sb.AppendLine("Desirable Preferences:");

        foreach (var d in r.Desirable)
        {
            sb.AppendLine($"  • {d.Country} ({d.Options})");
        }
    }
}