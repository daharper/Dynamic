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
        dynamic bob = NewPerson("Bob");

        var result = bob.HairColor("green");

        Assert.Equal("green", bob.Props["HairColor"]);
        Assert.Same(bob, result);
    }

    [Fact]
    public void Auto_Property_Throws_When_Disabled()
    {
        ActiveRuntime.AutoProperties = false;

        try
        {
            dynamic bob = NewPerson("bob");

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
