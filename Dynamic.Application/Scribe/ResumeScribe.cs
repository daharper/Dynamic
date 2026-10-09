using System.Text;
using Dynamic.Career;
using Dynamic.Runtime;

namespace Dynamic.Application.Scribe;

public abstract class ResumeScribe 
{
    protected static void WriteListed(ActiveMetadata o, StringBuilder sb)
    {
        if (!o.IsList) return;
        
        sb.AppendLine("    [lists]");

        foreach (var listed in o.AllListed)
        {
            sb.AppendLine($"  • {listed.Name}");
        }
    }
    
    protected static void WriteListings(ActiveMetadata o, StringBuilder sb)
    {
        if (!o.IsListed) return;
        
        sb.AppendLine("    [listed in]");

        foreach (var list in o.AllLists)
        {
            sb.AppendLine($"  • {list.Name}");
        }
    }
    
    protected static void WriteProperties(ActiveMetadata o, StringBuilder sb)
    {
        if (!o.AnyMetaProperty) return;
        
        sb.AppendLine("    [properties]");

        foreach (var prop in o.MetaProperties)
        {
            sb.AppendLine($"  • {prop.Key} = {o.AsStr(prop.Key)}");
        }
    }

    protected static void WriteNotes(ActiveMetadata o, StringBuilder sb)
    {
        if (!o.AnyMetaNote) return;
        
        sb.AppendLine("        [notes]");

        foreach (var note in o.MetaNotes)
        {
            sb.AppendLine($"        • {note}");
        }
    }
}