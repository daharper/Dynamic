using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class RoleRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Role;
    
    public void Register(Resume r)
    {
        r.Roles.HeuLabs
            .Title("Solution Architect")
            .StartYear(2005)
            .EndYear(2007)
            .Description("C# development in an award-winning EdTech referenced by Microsoft and UNESCO");
    }
}
