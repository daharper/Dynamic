using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class DetailRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Detail;
    
    public void Register(Resume r)
    {
        r.Name = "David Harper";
        r.Title = "Software Engineer | C# • Delphi";
        r.Email = "david@beyondvelocity.co.uk";
        r.LinkedIn = "https://www.linkedin.com/in/david-harper-82935b148/";
        r.GitHub = "https://github.com/daharper/"; 
        r.Location = "Spennymoor, County Durham, United Kingdom";
        r.Availability = Availability.Immediately;
        r.MinimumSalary = new Money(50_000m, "GBP");
    }
}