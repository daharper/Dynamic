using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class RecommendationScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Recommendation;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Recommendations:");

        foreach (var rec in r.Recommendations)
        {
            sb.AppendLine($"{rec.Name} - {rec.Title} ({rec.Company.Name})");
            sb.AppendLine($"{rec.LinkedIn}");
            sb.AppendLine();
            
            sb.AppendLine(rec.Description);
            
            if (rec.AnyMetaNote)
            {
                sb.AppendLine("[notes]");

                foreach (var note in rec.MetaNotes)
                {
                    sb.AppendLine(note);
                }
            }

            sb.AppendLine();
        }
    }
}
 