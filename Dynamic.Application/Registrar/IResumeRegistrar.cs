using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public interface IResumeRegistrar
{
    ResumeSection Section { get; }
    
    void Register(Resume r);
}