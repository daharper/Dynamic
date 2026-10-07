using Dynamic.Runtime;

namespace Dynamic.Career;

public class Task : ActiveObject<Task>
{
    public Task(Role role, string? description = null)
    {
        Description = description ?? "";
        Role = role;
    }

    public string Description { get; set; }
    public Role Role { get; set; } 
    
    public List<AppliedSkill> Skills { get; set; } = [];

    // public Task Add(params AppliedSkill[] skills)
    // {
    //     Skills.AddRange(skills);
    //     return this;
    // }
}