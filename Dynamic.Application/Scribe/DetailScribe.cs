using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class DetailScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Detail;

    public void Write(Resume r, StringBuilder sb)
    {
        var output = $"""
                      Name: {r.Name}
                      Title: {r.Title}
                      Email: {r.Email}
                      LinkedIn: {r.LinkedIn}
                      Location: {r.Location}
                      Availability: {r.Availability}
                      Minimum Salary: {r.MinimumSalary}

                      """;

        sb.AppendLine(output);
    }
}