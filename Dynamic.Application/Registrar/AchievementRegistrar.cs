using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class AchievementRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Achievement;
    
    public void Register(Resume r)
    {
        r.Achievements.Mvp.Organisation = "Embarcadero";
        
        r.Achievements.ArcticCodeVault("GitHub").Title = "Arctic Code Vault Contributor";
    }
}