using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class RoleRegistrar : IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Role;
    
    public void Register(Resume r)
    {
        WebSpy(r);
        
        // r.Roles.HeuLabs
        //     .Title("Solution Architect")
        //     .StartYear(2005)
        //     .EndYear(2007)
        //     .Description("C# development in an award-winning EdTech referenced by Microsoft and UNESCO");
    }

    private void WebSpy(Resume r)
    {
        r.Roles.WebSpy
            .Description(
                "Contributed to a specialized product suite (WebSpy Sentinel, WebSpy Live, and WebSpy Analyzer) " +
                "that was bundled with enterprise gateway platforms, including Microsoft ISA Server, to provide " +
                "small-to-medium enterprises (SMEs) with comprehensive web-traffic visibility.\n\n" +

                "WebSpy Analyzer was a sophisticated log-parsing and reporting engine that generated granular " +
                "management reports from historical proxy, firewall, and mail logs. It could optionally store " +
                "parsed data in a central database and query that data for reporting.\n\n" +

                "WebSpy Analyzer evolved from its original focus on Squid proxy logs into a vendor-independent " +
                "analysis platform, eventually supporting more than 250 log formats from over 128 hardware and " +
                "software vendors.\n\n" +

                "WebSpy Sentinel captured copies of raw network packets directly from network interfaces in " +
                "real time using kernel-level device drivers, feeding traffic data associated with HTTP, FTP, POP3, " +
                "and SMTP into a central MySQL database or log file. WebSpy Analyzer could also be configured " +
                "to use the same database.\n\n" +

                "WebSpy Live actively polled the database or monitored the log file, raising alerts based on " +
                "configurable triggers.")
            .Title("Delphi Developer")
            .StartYear(1999)
            .EndYear(2000)
            .Lists(
                r.Skills.Delphi, r.Skills.CrystalReports, r.Skills.DevExpress, r.Skills.SQL, r.Skills.MySQL,
                r.Skills.VCL, r.Skills.BDE, r.Skills.HTML, r.Skills.ObjectPascal)
            .Lists(r.Recommendations.WebSpy);
    }
}
