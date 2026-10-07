using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class PreferenceRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Preference;
    
    public void Register(Resume r)
    {
        r.Eligible.Australia.Options = Option.Remote | Option.AnyOnRelocation;
        r.Eligible.Ireland.Options = Option.Remote;
        r.Eligible.UnitedKingdom.Options = Option.Remote;
        r.Eligible.NorthEastUnitedKingdom.Options = Option.Remote | Option.Hybrid;

        r.Desirable.Singapore.Options = Option.Remote | Option.AnyOnRelocation;
        r.Desirable.USA.Options = Option.Remote | Option.AnyOnRelocation;
    }
}
