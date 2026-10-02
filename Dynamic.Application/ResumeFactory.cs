using System;
using System.Collections.Generic;
using System.Text;
using Dynamic.Application.Registrar;
using Dynamic.Career;

namespace Dynamic.Application;

public static class ResumeFactory
{
    private static readonly List<IResumeRegistrar> Registrars =
    [
        new WorkPreferencesRegistrar(),
        new CompanyRegistrar(),
        new HighlightsRegistrar()
    ];

    public static Resume Create()
    {
        var r = new Resume
        {
            Name = "David Harper",
            Title = "Software Engineer | C# • Delphi",
            Email = "david@beyondvelocity.co.uk",
            LinkedIn = "https://www.linkedin.com/in/david-harper-82935b148/",
            Location = "Spennymoor, County Durham, United Kingdom",
            Availability = Availability.Immediately,
            MinimumSalary = new Money(50_000m, "GBP")
        };

        foreach (var registrar in Registrars)
        {
            registrar.Register(r);
        }

        return r;
    }
}
