using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class CompanyScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Company;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Companies:");

        foreach (var c in r.Companies)
        {
            sb.AppendLine($"{c.Title} ({c.Scale}, {c.Country})");
            sb.AppendLine(c.Description);
            sb.AppendLine();

            if (c.HasNote)
            {
                sb.AppendLine("[notes]");

                foreach (var note in c.Notes)
                {
                    sb.AppendLine(note);
                }
            }

            sb.AppendLine();
        }
    }
}