using System.Text;
using Dynamic.Career;

namespace Dynamic.Application.Scribe;

public interface IResumeScribe
{
    ResumeSection Section { get; }
    
    void Write(Resume r, StringBuilder sb);
}