using System.Linq.Expressions;
using Dynamic.Runtime;
using Dynamic.Career;

ActiveRuntime.Register(typeof(Person));

dynamic alice = new Person { FirstName = "Alice" };
dynamic bob = new Person { FirstName = "Bob" };

bob.Eval("""
         public Expression<Func<Person, bool>> FirstNameIs(string name)
         {
             return person => person.FirstName == name;
         }
         
         public Expression<Func<Person, bool>> MinimumExperience(int years)
         {
             return person => person.Years >= years;
         }
         """);

Expression<Func<Person, bool>> firstNameIsBob = bob.FirstNameIs("Bob");

Expression<Func<Person, bool>> minimumExperience = bob.MinimumExperience(5);

var firstNameSpecification = SpecificationExtractor.Extract(firstNameIsBob);

var experienceSpecification = SpecificationExtractor.Extract(minimumExperience);

Console.WriteLine(firstNameSpecification);
Console.WriteLine(experienceSpecification);