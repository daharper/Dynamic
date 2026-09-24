using System.Reflection;
using Dynamic.Runtime;
using Dynamic.Tests.Mocks;

namespace Dynamic.Tests.Fixtures;

public abstract class RuntimeTestBase
{
    protected RuntimeTestBase()
    {
        ResetPersonRuntime();
    }

    protected static dynamic NewPerson(string firstName)
        => new Person
        {
            FirstName = firstName
        };

    private static void ResetPersonRuntime()
    {
        var field =
            typeof(ActiveClassRegistry<Person>)
                .GetField(
                    "_current",
                    BindingFlags.Static |
                    BindingFlags.NonPublic);

        if (field is null)
        {
            throw new InvalidOperationException(
                "Could not locate ActiveClassRegistry<Person>._current. " +
                "If the registry implementation changes, update the test reset helper.");
        }

        field.SetValue(
            null,
            ActiveClass<Person>.Empty);
    }

    [Fact]
    public void Runtime_method_takes_precedence_over_clr_method()
    {
        dynamic alice =
            NewPerson("Alice");

        alice.ClassEval("""
                        public string ToString()
                        {
                            return "Runtime";
                        }
                        """);

        Assert.Equal(
            "Runtime",
            (string)alice.Send("ToString"));
    }
}
