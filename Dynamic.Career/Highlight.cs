using Dynamic.Runtime;

namespace Dynamic.Career;

public class Highlight(Company company, string description = "") : ActiveObject<Highlight>
{
    public string Name => Company.Name;
    
    public string Description { get; set; } = description;

    public Company Company { get; set; } = company;
}
