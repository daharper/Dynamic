using System.Text;
using Dynamic.Application.Scribe;
using Dynamic.Career;

namespace Dynamic.Application;

/// <summary>
/// Provides functionality for generating and writing résumés sections for diagnostic purposes.
/// This class uses a list of implementations of <see cref="IResumeScribe"/>
/// to process and format specific sections of a résumé based on the specified
/// <see cref="ResumeSection"/> flags.
/// </summary>
public static class ResumeWriter
{
    private static readonly List<IResumeScribe> Scribes =
    [
        new DetailScribe(),
        new PreferenceScribe(),
        new AchievementScribe(),
        new CompanyScribe(),
        new HighlightScribe(),
        new SkillScribe(),
        new RecommendationScribe(),
        new RoleScribe(),
    ];
    
    public static void Write(Resume r, ResumeSection sections)
    {
        var sb = new StringBuilder();
        
        foreach (var scribe in Scribes)
        {
            if ((sections & scribe.Section) == scribe.Section)
            {
                scribe.Write(r, sb);
            }
        }

        Console.WriteLine(sb.ToString());
    }
}