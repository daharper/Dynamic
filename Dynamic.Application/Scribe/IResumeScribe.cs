using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

[Flags]
public enum ResumeSection
{
    Company = 1,
    All = 255
}

public interface IResumeScribe
{
    ResumeSection Section { get; }
    
    void Write(Resume r, StringBuilder sb);
}