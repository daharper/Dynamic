using Dynamic.Runtime;

namespace Dynamic.Tests.Fixtures;

public class ActiveRuntimeTests : RuntimeTestBase
{
    [Fact]
    public void AutoProperties_Defaults_To_Enabled()
    {
        Assert.True(ActiveRuntime.AutoProperties);
    }

    [Fact]
    public void AutoProperties_Can_Be_Disabled()
    {
        ActiveRuntime.AutoProperties = false;

        Assert.False(ActiveRuntime.AutoProperties);

        ActiveRuntime.AutoProperties = true;
    }

    [Fact]
    public void Unknown_Member_Invocation_Creates_Auto_Property()
    {
        var bob = NewPerson("Bob");

        var result = bob.HairColor("green");

        Assert.Equal("green", bob.Props["HairColor"]);
        Assert.Same(bob, result);
    }


    [Fact]
    public void Unknown_Member_Invocation_For_Known_Property()
    {
        var bob = NewPerson("Bob");

        var result = bob.FirstName("Bobby");

        Assert.Equal("Bobby", bob.FirstName);
        Assert.Same(bob, result);
    }

    [Fact]
    public void Known_inherited_property_can_be_invoked_fluently()
    {
        var bob = NewPerson("Bob");

        var result = bob.Description("Hello");

        Assert.Equal("Hello", bob.Description);
        Assert.Same(bob, result);
    }

    [Fact]
    public void Auto_Property_Throws_When_Disabled()
    {
        ActiveRuntime.AutoProperties = false;

        try
        {
            var bob = NewPerson("bob");

            var exception = Assert.Throws<InvalidOperationException>(() => bob.HairColor("green"));

            Assert.Contains("HairColor", exception.Message);
            Assert.False(bob.Props.ContainsKey("HairColor"));
        }
        finally
        {
            ActiveRuntime.AutoProperties = true;
        }
    }
}
