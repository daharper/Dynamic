using Dynamic.Application.Registrar;
using Dynamic.Career;

namespace Dynamic.Application;

public static class ResumeFactory
{
    private static readonly List<IResumeRegistrar> Registrars =
    [
        new DetailsRegistrar(),
        new WorkPreferencesRegistrar(),
        new CompanyRegistrar(),
        new HighlightsRegistrar(),
        new RoleRegistrar()
    ];

    public static Resume Create()
    {
        var r = new Resume();
        
        foreach (var registrar in Registrars)
        {
            registrar.Register(r);
        }

        return r;
    }
}
