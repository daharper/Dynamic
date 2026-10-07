using Dynamic.Application.Registrar;
using Dynamic.Career;

namespace Dynamic.Application;

/// <summary>
/// Factory class for creating instances of the <see cref="Resume"/> class.
/// This class aggregates multiple registrars that populate different sections of a résumé
/// and applies them during the creation process.
/// </summary>
public static class ResumeFactory
{
    private static readonly List<IResumeRegistrar> Registrars =
    [
        new DetailRegistrar(),
        new PreferenceRegistrar(),
        new AchievementRegistrar(),
        new CompanyRegistrar(),
        new HighlightRegistrar(),
        new SkillRegistrar(),
        new RecommendationRegistrar(),
        new RoleRegistrar()
    ];

    public static Resume Create(ResumeSection sections)
    {
        var r = new Resume();
        
        foreach (var registrar in Registrars)
        {
            if ((sections & registrar.Section) == registrar.Section)
            {
                registrar.Register(r);
            }
        }

        return r;
    }
}
