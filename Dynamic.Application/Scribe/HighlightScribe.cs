using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class HighlightScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Highlight;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Highlights:");

        foreach (var h in r.Highlights)
        {
            sb.AppendLine($"  • {h.Company.Name}: {h.Description}");

            if (!h.HasLink) continue;
            
            foreach (var link in h.Links)
            {
                sb.AppendLine($"    - {link.Title} ({link.Url})");
            }
        }
    }
}