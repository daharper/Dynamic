using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class HighlightsRegistrar : IResumeRegistrar
{
    public void Register(Resume r)
    {
        r.Highlights.HeuLabs.Description(
                "Presented HeuLabs' patented C# Tablet PC-based virtual classroom, HeuCampus, live on stage with CEO Rakesh Gupta " +
                "at Microsoft Singapore's official Visual Studio 2005 launch event; cited in a UNESCO ICT report and Microsoft " +
                "case study, later highlighted by Bill Gates in his keynote at the 2007 Microsoft Government Leaders Forum — Asia")
            .Link("UNESCO ICT report", "https://unesdoc.unesco.org/ark:/48223/pf0000211842");

        r.Highlights.HeuLabs.Description(
                "Led development of HeuCampus in C#/WinForms, and built a C#/Mono/XMPP component framework with supporting " +
                "backend services powering real-time messaging across schools and universities via Windows, Linux & FreeBSD servers");

        r.Highlights.IndependentRnD.Description(
                "Built a modern Delphi 13.1 library supporting Clean Architecture and DDD, including dependency injection " +
                "and host composition, provider-agnostic data abstractions, ambient sessions, specifications, functional " +
                "types like Result and Option, an eager Java-like stream, segment, slice, index, detached views, scope " +
                "management, dynamic dispatch with method missing, dynamic proxy, an event bus, XML parser, and more")
            .Link("The Delphi Way", "https://beyondpotency.com/post/39");

        r.Highlights.Singtel.Description(
                "Modernised a Java WebLogic application to Spring Boot using a strangler-fig approach; promoted to lead " +
                "Digital Identity & One Pass squads");

        r.Highlights.CreditSuisse.Description(
                "Rescued and delivered a C# project for settling trades on Bursa Malaysia, reducing an 8-month timeline " +
                "to 10 weeks and saving ~$350k p.a. Owned end-to-end delivery across requirements, architecture, " +
                "implementation, testing, and release in a regulated financial environment");

        r.Highlights.MoneyHero.Description(
                "Core Kotlin and Java 17 developer on a next-gen cloud-native fintech platform using Quarkus, Hibernate " +
                "and Kafka; built annotation-driven code generation and microservices for event ingestion and dynamic " +
                "Kafka routing; assisted Spring Boot teams migrating to the platform");

        r.Highlights.STEngineering.Description(
                "Led C++ backend development on Beacon World, a virtual campus built for the FutureSchools@Singapore " +
                "national initiative led by MOE and IDA; demonstrated to international delegates and featured as a case " +
                "study in UNESCO's report on ICT in primary education");

        r.Highlights.Intact.Description(
                "At Intact Software, developed a C# 13/DevExpress Roslyn scripting IDE, enabling LINQ queries across " +
                "multiple 850-file legacy DBF customer datasets and filtering and grouping in DevExpress " +
                "grids—replacing days/weeks of bespoke development with 10-minute scripts");

        r.Highlights.HeuLabs.Description(
                "Introduced C# and Avalon (later WPF) to Microsoft's Partner Network in Singapore and provided a live coding demonstration");

        r.Highlights.STEngineering.Description(
                "Managed and mentored three interns at ST Engineering, turning around an initially struggling team; " +
                "directed Agile C# projects and liaised with their professor through to successful completion of all deliverables");

        r.Highlights.IndependentRnD.Description(
                "Developed a Delphi/C# developer workbench for a 2M-line legacy codebase, transforming AI-enriched " +
                "AST metadata into a SQL Server semantic model with C#/LINQ analysis, UML generation, IDE automation " +
                "via a Delphi OTA plugin, and Win32-hosted TCP/IP plugins");

        r.Highlights.STEngineering.Description(
                "Built a declarative C++ data-access DSL in one week upon urgent request, using templates and preprocessor " +
                "metaprogramming over ADOX, generating CRUD, typed properties and keys, parameterised queries, and collection " +
                "materialisation from concise entity/query declarations");

        r.Highlights.EQUE2.Description(
                "Used Ruby metaprogramming on Test::Unit to supply fixtures and Sage/Xero integration testing, and built " +
                "configurable TracePoint tracing");

        r.Highlights.STEngineering.Description(
                "Introduced automated testing to a native C++ codebase by using C++/CLI to bridge into NUnit, with reusable " +
                "fixtures and managed/native marshalling. Built an isolated SQL Server test environment to provide " +
                "deterministic integration testing without modifying production code");

        r.Highlights.Minemax.Description(
                "Core developer modernising Scheduler from C++Builder to C#/WPF/DDD; built C++/CLI interop bridging C/IBM CPLEX to C# services");

        r.Highlights.STEngineering.Description(
                "At ST Engineering, built a C# model-driven XML DSL and pluggable code generator, transforming entity/relationship " +
                "models into an object graph used to generate complete C++ data layers and APIs, SQL Server DDL and seed data, " +
                "and Visual Studio project updates");

        r.Highlights.RWWA.Description(
                "Built a C#/WCF/XMPP tool pushing 300 TPS of historical Melbourne Cup data from Oracle across C++ AIX " +
                "services behind an OSB SOA");
    }
}
