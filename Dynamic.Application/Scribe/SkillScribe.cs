using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class SkillScribe : ResumeScribe, IResumeScribe
{
    public ResumeSection Section => ResumeSection.Skill;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Skills:");
        sb.AppendLine();

        foreach (var s in r.Skills)
        {
            WriteNameAndAliases(s, sb);
            
            WriteListed(s, sb);
            WriteListings(s, sb);

            WriteNotes(s, sb);
            WriteProperties(s, sb);

            sb.AppendLine();
        }
    }

    private static void WriteNameAndAliases(Skill s, StringBuilder sb)
    {
        var aliases = s.AnyMetaAlias ? "(" + string.Join(" ", s.MetaAliases) + ")" : "";

        sb.AppendLine($"  • {s.Title} {aliases}");
    }
}