namespace Dynamic.Application;

[Flags]
public enum ResumeSection
{
    Detail = 1,
    Company = 2,
    Preference = 4,
    Highlight = 8,
    Role = 16,
    Achievement = 32,
    Skill = 64,
    Recommendation = 128,
    All = 255,
}