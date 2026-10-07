using Dynamic.Career;

namespace Dynamic.Application.Registrar;

public class RecommendationRegistrar :  IResumeRegistrar
{
    public ResumeSection Section => ResumeSection.Recommendation;
    
    public void Register(Resume r)
    {
        r.Recommendations.WebSpy
            .Name("Michael Thompson")
            .Title("General Manager")
            .LinkedIn("https://www.linkedin.com/in/michael-thompson-69759a7/")
            .Description(
                "David subcontracted for us around 1999 to 2000, and despite the time that’s passed, " +
                "I still remember him clearly, which speaks to me.\n\n" +
                "I have no hesitation in recommending David, both for his technical capability and for " +
                "the kind of colleague he is. \n\n" +
                "He was a highly competent programmer who consistently delivered. At the time, he worked extensively " +
                "in Delphi, often tackling problems where documentation was sparse and answers weren't a quick " +
                "search away. One example that stuck with me was his work on in-memory compression routines, " +
                "built largely from raw specifications - and he delivered reliable, working solutions.\n\n" +
                "What I remember most, though, was David’s personal integrity. He was someone who cared about " +
                "the people he worked with and the environment he worked in. On more than one occasion he was " +
                "willing to give additional time and effort beyond what he was contracted for, simply because " +
                "he believed it was the right thing to do for the team.");
    }
}