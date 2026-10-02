using Dynamic.Runtime;

namespace Dynamic.Career;

public class Highlight : ActiveObject<Highlight>
{
    public Highlight(Company company, string description = "") : base(company.Name)
    {
        Company = company;
        Description = description;
    }

    public Company Company { get; set; }
}
