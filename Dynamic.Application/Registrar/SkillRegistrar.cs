using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class SkillRegistrar : IResumeRegistrar
{
    public void Register(Resume r)
    {
        DefineSkills(r);
        ClassifySkills(r);
    }
    
    private void DefineSkills(Resume r)
    {
        var s = r.Skills;

        s.CSharp
            .Alias("C#")
            .Let("Versions", Enumerable.Range(1, 15).Select(i => i.ToString()).ToList())
            .Note("First used C# in 2001 whilst in beta");
    }
    
    private void ClassifySkills(Resume r)
    {
    }
}