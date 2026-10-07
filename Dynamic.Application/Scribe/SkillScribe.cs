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
            // WriteParent(s, sb);
            // WriteChildren(s, sb);
            
            WriteListed(s, sb);
            WriteListings(s, sb);

            WriteNotes(s, sb);
            WriteProperties(s, sb);
        }
    }

    private static void WriteNameAndAliases(Skill s, StringBuilder sb)
    {
        var aliases = s.AnyMetaAlias ? "(" + string.Join(" ", s.MetaAliases) + ")" : "";

        sb.AppendLine($"  • {s.Name} {aliases}");
    }
    
    // private static void WriteParent(Skill s, StringBuilder sb)
    // {
    //     if (!s.HasParent) return;
    //     
    //     sb.AppendLine("        [parent]");
    //     sb.AppendLine($"        • {s.Parent!.Name}");
    // }
    //
    // private static void WriteChildren(Skill s, StringBuilder sb)
    // {
    //     if (!s.HasChild) return;
    //     
    //     sb.AppendLine("        [children]");
    //
    //     foreach (var child in s.Children)
    //     {
    //         sb.AppendLine($"        • {child.Name}");
    //     }
    // }
    
    private static void WriteListed(Skill s, StringBuilder sb)
    {
        if (!s.IsList) return;
        
        sb.AppendLine("        [lists]");

        foreach (var listed in s.AllListed)
        {
            sb.AppendLine($"        • {listed.Name}");
        }
    }
    
    private static void WriteListings(Skill s, StringBuilder sb)
    {
        if (!s.IsListed) return;
        
        sb.AppendLine("        [listed in]");

        foreach (var list in s.AllLists)
        {
            sb.AppendLine($"        • {list.Name}");
        }
    }
    
    private static void WriteProperties(Skill s, StringBuilder sb)
    {
        if (!s.AnyMetaProperty) return;
        
        sb.AppendLine("        [properties]");

        foreach (var prop in s.MetaProperties)
        {
            sb.AppendLine($"        • {prop.Key} = {s.AsStr(prop.Key)}");
        }
    }

    private static void WriteNotes(Skill s, StringBuilder sb)
    {
        if (!s.AnyMetaNote) return;
        
        sb.AppendLine("        [notes]");

        foreach (var note in s.MetaNotes)
        {
            sb.AppendLine($"        • {note}");
        }
    }
}