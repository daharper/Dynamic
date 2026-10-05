using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class RoleScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Role;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Roles:");

        foreach (var role in r.Roles)
        {
            sb.AppendLine($"  • {role.StartYear} - {role.EndYear} {role.Company.Name} {role.Title}");
            sb.AppendLine($"    {role.Description}");
        }
    }
}