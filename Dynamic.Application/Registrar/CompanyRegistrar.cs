using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class CompanyRegistrar : IResumeRegistrar
{
    public void Register(Resume r)
    {
        r.Companies.IndependentRnD("Self-funded research and development")
            .Scale(CompanyScale.None)
            .Country(Country.UnitedKingdom)
            .Title = "Independent Research & Development";

        r.Companies.GDKSoftware(
                "GDK Software is a global software development company and a recognized authority " + 
                "on the Delphi programming language. Founded in 2006 and based in Papendrecht, Netherlands, " + 
                "the company operates across dozens of countries to help businesses maintain, upgrade, " + 
                "and modernize complex legacy applications.")
            .Scale(CompanyScale.Small)
            .Country(Country.UnitedKingdom)
            .Title = "GDK Software UK";
        
        r.Companies.Intact(
                "A leading global developer of cloud-native and on-premise Enterprise Resource Planning(ERP) " +
                "and business management software. The company delivers highly customizable, end-to-end " +
                "digital transformation platforms tailored specifically to optimize operations, finances, and " +
                "supply chains for mid - market merchants, wholesalers, and distributors.")
            .Scale(CompanyScale.Medium)
            .Note("Rebranded as GenetiQ")
            .Country(Country.UnitedKingdom)
            .Title = "Intact Software UK";

        r.Companies.EQUE2(
                "A market-leading provider of specialist, cloud-based ERP, contract management, and estimating " +
                "software purpose-built for the construction, contracting, and housebuilding industries. The " +
                "company serves over 3,000 businesses—ranging from mid-market contractors to major enterprise " +
                "groups—by integrating commercial, financial, and project data to streamline compliance and " +
                "protect construction margins.")
            .Scale(CompanyScale.Medium)
            .Country(Country.UnitedKingdom);
        
        r.Companies.BeyondVelocity(
                "A .NET consultancy, providing C# services to clients across Singapore and Australia, whilst " + 
                "pursuing product development in Delphi.")
            .Scale(CompanyScale.Startup)
            .Country(Country.Australia)
            .Title = "Beyond Velocity Pty Ltd";

        r.Companies.MoneyHero(
                "A leading NASDAQ-listed fintech enterprise and the premier online personal finance aggregation and comparison " +
                "platform in Greater Southeast Asia. The company operates a portfolio of dominant market brands—including" +
                "SingSaver, MoneyHero, and Money101—connecting millions of monthly consumers with comprehensive digital tools " +
                "to discover, compare, and apply for banking, insurance, and investment products.")
            .Scale(CompanyScale.Enterprise)
            .Note("Formerly known as Hyphen Group")
            .Country(Country.Singapore)
            .Title = "MoneyHero Group";
        
        r.Companies.Singtel(
                "A leading global communications technology group and Asia’s premier telecommunications enterprise. " +
                "Headquartered in Singapore, the company provides an extensive portfolio of next - generation 5G " +
                "connectivity, digital infrastructure, and enterprise technology services—including cloud orchestration" +
                ", cybersecurity, and data centres—serving millions of consumers and corporate clients across Asia, " +
                "Australia, and international markets.")
            .Scale(CompanyScale.Enterprise)
            .Country(Country.Singapore);

        r.Companies.CreditSuisse(
                "A leading global wealth management and investment banking enterprise, operating as a primary regional division " +
                "of the Swiss-headquartered multinational financial institution. Prior to its comprehensive integration into UBS, " +
                "the Asia-Pacific franchise managed over CHF 200 billion in assets under management and delivered end-to-end capital " +
                "markets, advisory, and tailored financing solutions to institutional, corporate, and ultra-high-net-worth clients " +
                "across the region.")
            .Scale(CompanyScale.Enterprise)
            .Country(Country.Singapore)
            .Note("Credit Suisse was acquired by UBS in 2023")
            .Title = "Credit Suisse APAC)";
        
        r.Companies.Minemax(
                "A premier Australian mining software development enterprise specializing in advanced strategic mine " +
                "planning and schedule optimization solutions.Headquartered in Perth, the company's core " +
                "platform—Minemax Scheduler—utilizes powerful mathematical optimization engines to jointly " +
                "evaluate extraction sequences, logistics, and capital expenditure constraints, maximizing net " +
                "present value (NPV) for global mining operators across more than 35 countries.")
            .Scale(CompanyScale.Small)
            .Country(Country.Australia)
            .Note("Minemax was acquired by Datamine");

        r.Companies.STEngineering(
                "A global technology engineering group headquartered in Singapore, delivering innovative solutions " +
                "across the aerospace, electronics, land systems, and marine sectors. The company provides a wide " +
                "range of products and services—including defence systems, smart city solutions, and advanced " +
                "engineering technologies—to customers in over 100 countries.")
            .Scale(CompanyScale.Enterprise)
            .Country(Country.Singapore)
            .Title = "ST Engineering";

        r.Companies.RWWA(
                "A major state - owned gaming, wagering, and sports administration enterprise responsible for the " +
                "strategic direction, integrity, and economic development of the racing industry in Western Australia. " +
                "The statutory corporation operates the state's off-course totalisator (TAB) and retail betting network, " +
                "managing comprehensive commercial wagering operations, regulatory compliance, and distribution " +
                "infrastructure supporting greyhound, harness, and thoroughbred racing codes.")
            .Scale(CompanyScale.Enterprise)
            .Country(Country.Australia)
            .Title = "Racing and Wagering Western Australia";

        r.Companies.Bankwest(
                "A leading Australian financial services enterprise operating as a prominent division of the " +
                "Commonwealth Bank of Australia.Headquartered in Perth, the institution delivers a comprehensive " +
                "portfolio of personal, business, and commercial banking solutions—with a strategic focus on " +
                "scale, digital - first retail experiences, and simplified home lending infrastructure—serving " +
                "more than one million customers nationwide.")
            .Scale(CompanyScale.Enterprise)
            .Country(Country.Australia)
            .Note("Bankwest was acquired by Commonwealth Bank of Australia in 2008");

        r.Companies.RippleSystems(
                "A leading provider of mission-critical control systems and automation software specializing in mass " +
                "rapid transit (MRT) and heavy industrial infrastructure. Prior to its acquisition by ST Engineering, " +
                "the company operated as the core engineering design and systems integration team responsible for " +
                "developing the Integrated Supervisory and Control Systems (ISCS) and advanced communication " +
                "architectures for the Singapore MRT Circle Line.")
            .Scale(CompanyScale.Small)
            .Country(Country.Australia)
            .Note("Ripple Systems was acquired by ST Engineering")
            .Title = "Ripple Systems";
        
        r.Companies.HeuLabs(
                "An innovative educational and mobile applications software development provider specializing in " +
                "interactive digital learning platforms and tablet - optimized software.The company partnered with " +
                "content academic institutions and global technology vendors to deploy customizable digital note - taking, " +
                "consumption, and interactive classroom solutions across more than 140 schools internationally.")
            .Scale(CompanyScale.Startup)
            .Country(Country.Singapore);

        r.Companies.Accenture(
                "A leading global professional services enterprise delivering a comprehensive range of consulting, technology, " +
                "and outsourcing solutions. The company partners with clients across industries to drive digital transformation, " +
                "optimize business processes, and implement innovative technology solutions, leveraging its extensive expertise " +
                "in strategy, consulting, digital, technology, and operations.")
            .Scale(CompanyScale.Enterprise)
            .Country(Country.Singapore)
            .Title = "Accenture APAC (Asia Pacific and China)";

        r.Companies.HandiSoft(
                "A leading provider of integrated taxation, accounting, and practice management software solutions purpose-built " +
                "for public practice accountants and tax agents across Australia. Now a key division of The Access Group, the " +
                "company delivers end-to-end compliance, workflow automation, and electronic lodgement infrastructure supporting " +
                "thousands of professional accounting firms nationwide.")
            .Scale(CompanyScale.Medium)
            .Country(Country.Australia)
            .Note("HandiSoft was acquired by The Access Group in 2021");

        r.Companies.Bloodhound(
                "A leading provider of cloud - based payment integrity, data analytics, and real - time claims editing software " +
                "designed to mitigate fraud, waste, and abuse within the US healthcare market.Through its specialized, cross-border " +
                "research and development infrastructure, the company engineered high - throughput SaaS platforms utilizing advanced " +
                "predictive analytics to intercept billing anomalies, multi - party provider non - compliance, and aberrant claim " +
                "patterns prior to commercial adjudication")
            .Scale(CompanyScale.Medium)
            .Country(Country.Australia)
            .Note("Bloodhound Technologies was acquired by Verisk Analytics")
            .Title = "Bloodhound Technologies";

        r.Companies.Algarburns(
                "A synchronized regional telecommunications, digital media, and internet service provider (ISP) delivering integrated " +
                "connectivity and web infrastructure solutions across Western Australia. Operating as dual entities under unified " +
                "ownership in Osborne Park, the enterprise engineered high-speed broadband network deployments and business " +
                "connectivity pipelines, while simultaneously developing early-stage mobile-optimized websites and digital hosting " +
                "architectures for commercial clients across metropolitan Perth.")
            .Country(Country.Australia)
            .Scale(CompanyScale.Small);

        r.Companies.WebSpy(
                "A pioneering cyber-safety and network analytics software developer specializing in internet, email, and network " +
                "usage reporting solutions. Headquartered in Osborne Park, Western Australia, the company engineered enterprise " +
                "log-file analysis platforms—most notably WebSpy Vantage—that aggregate and cross-reference data from over 200 " +
                "security proxy and firewall vendors to help organizations mitigate organizational risk, monitor compliance, and " +
                "optimize workforce productivity.")
            .Country(Country.Australia)
            .Scale(CompanyScale.Small);
    }
}
