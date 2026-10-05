using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public class SkillScribe : IResumeScribe
{
    public ResumeSection Section => ResumeSection.Skill;

    public void Write(Resume r, StringBuilder sb)
    {
        sb.AppendLine("Skills:");

        foreach (var s in r.Skills)
        {
            WriteNameAndAliases(s, sb);
            WriteParent(s, sb);
            WriteChildren(s, sb);
            WriteMembers(s, sb);
            WriteMemberships(s, sb);
            WriteNotes(s, sb);
            WriteProperties(s, sb);
        }
    }

    private static void WriteNameAndAliases(Skill s, StringBuilder sb)
    {
        var aliases = s.HasAlias ? "(" + string.Join(" ", s.Aliases) + ")" : "";

        sb.AppendLine($"  • {s.Name} {aliases}");
    }
    
    private static void WriteParent(Skill s, StringBuilder sb)
    {
        if (!s.HasParent) return;
        
        sb.AppendLine("        [parent]");
        sb.AppendLine($"        • {s.Parent!.Name}");
    }
    
    private static void WriteChildren(Skill s, StringBuilder sb)
    {
        if (!s.HasChild) return;
        
        sb.AppendLine("        [children]");

        foreach (var child in s.Children)
        {
            sb.AppendLine($"        • {child.Name}");
        }
    }
    
    private static void WriteMembers(Skill s, StringBuilder sb)
    {
        if (!s.HasMember) return;
        
        sb.AppendLine("        [members]");

        foreach (var member in s.AllMembers)
        {
            sb.AppendLine($"        • {member.Name}");
        }
    }
    
    private static void WriteMemberships(Skill s, StringBuilder sb)
    {
        if (!s.IsMember) return;
        
        sb.AppendLine("        [member of]");

        foreach (var memberOf in s.MemberOf)
        {
            sb.AppendLine($"        • {memberOf.Name}");
        }
    }
    
    private static void WriteProperties(Skill s, StringBuilder sb)
    {
        if (!s.HasProp) return;
        
        sb.AppendLine("        [properties]");

        foreach (var prop in s.Props)
        {
            sb.AppendLine($"        • {prop.Key} = {s.AsStr(prop.Key)}");
        }
    }

    private static void WriteNotes(Skill s, StringBuilder sb)
    {
        if (!s.HasNote) return;
        
        sb.AppendLine("        [notes]");

        foreach (var note in s.Notes)
        {
            sb.AppendLine($"        • {note}");
        }
    }
}