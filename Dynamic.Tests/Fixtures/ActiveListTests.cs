using Dynamic.Runtime;
using Dynamic.Tests.Mocks;

namespace Dynamic.Tests.Fixtures;

public class ActiveListTests
{
    [Fact]
    public void GetMember_AddsValue()
    {
        dynamic person = new Person();

        string friend = person.Friends.Bob;

        Assert.Equal("Bob", friend);
        Assert.Contains("Bob", person.Friends);
    }

    [Fact]
    public void GetMember_IsIdempotent()
    {
        dynamic person = new Person();

        string first = person.Friends.Bob;
        string second = person.Friends.Bob;

        Assert.Equal("Bob", first);
        Assert.Equal("Bob", second);
        Assert.Single(person.Friends);
    }

    [Fact]
    public void SetMember_AddsValue()
    {
        dynamic person = new Person();

        person.Friends.Bob = "Bob";

        Assert.Contains("Bob", person.Friends);
        Assert.Single(person.Friends);
    }

    [Fact]
    public void SetMember_IsIdempotent()
    {
        dynamic person = new Person();

        person.Friends.Bob = "Bob";
        person.Friends.Bob = "Bob";

        Assert.Single(person.Friends);
        Assert.Equal("Bob", person.Friends[0]);
    }

    [Fact]
    public void GetThenSet_IsIdempotent()
    {
        dynamic person = new Person();

        string bob = person.Friends.Bob;
        person.Friends.Bob = "Bob";

        Assert.Single(person.Friends);
        Assert.Equal("Bob", person.Friends[0]);
    }

    [Fact]
    public void SetThenGet_IsIdempotent()
    {
        dynamic person = new Person();

        person.Friends.Bob = "Bob";
        string bob = person.Friends.Bob;

        Assert.Equal("Bob", bob);
        Assert.Single(person.Friends);
    }

    [Fact]
    public void SetMember_AddsDifferentValues()
    {
        dynamic person = new Person();

        person.Friends.Bob = "Bob";
        person.Friends.Alice = "Alice";

        Assert.Equal(2, person.Friends.Count);
        Assert.Contains("Bob", person.Friends);
        Assert.Contains("Alice", person.Friends);
    }

    [Fact]
    public void IListOperations_UseConfiguredComparer()
    {
        dynamic friends = new ActiveList<string>(comparer: StringComparer.OrdinalIgnoreCase);

        friends.Add("Bob");

        Assert.True(friends.Contains("BOB"));
        Assert.Equal(0, friends.IndexOf("BOB"));
        Assert.True(friends.Remove("BOB"));
        Assert.Empty(friends);
    }

    [Fact]
    public void InvokeMember_AddsAchievement()
    {
        var david = new Person();

        Achievement mvp =
            david.Achievements.Mvp("Embarcadero");

        Assert.Equal(
            new Achievement("Mvp", "Embarcadero"),
            mvp);

        Assert.Single(david.Achievements);
        Assert.Same(mvp, david.Achievements[0]);
    }

    [Fact]
    public void InvokeMember_IsIdempotent()
    {
        var david = new Person();

        Achievement first =
            david.Achievements.Mvp("Embarcadero");

        Achievement second =
            david.Achievements.Mvp("Embarcadero");

        Assert.Single(david.Achievements);

        Assert.Same(first, second);
        Assert.Same(first, david.Achievements[0]);
    }

    [Fact]
    public void GetMember_UsesFactory()
    {
        var david = new Person();

        Achievement mvp = david.Achievements.Mvp;

        Assert.Equal(
            new Achievement("Mvp"),
            mvp);

        Assert.Single(david.Achievements);
        Assert.Same(mvp, david.Achievements[0]);
    }

    [Fact]
    public void InvokeThenGet_AddsDistinctAchievement()
    {
        var david = new Person();

        Achievement detailed =
            david.Achievements.Mvp("Embarcadero");

        Achievement unspecified =
            david.Achievements.Mvp;

        Assert.Equal(2, david.Achievements.Count);

        Assert.Equal(
            new Achievement("Mvp", "Embarcadero"),
            detailed);

        Assert.Equal(
            new Achievement("Mvp"),
            unspecified);
    }

    [Fact]
    public void SetMember_AddsAchievement()
    {
        var david = new Person();

        var mvp =
            new Achievement("Mvp", "Embarcadero");

        david.Achievements.Mvp = mvp;

        Assert.Single(david.Achievements);
        Assert.Same(mvp, david.Achievements[0]);
    }

    [Fact]
    public void SetMember_Obj_IsIdempotent()
    {
        var david = new Person();

        var first =
            new Achievement("Mvp", "Embarcadero");

        var equivalent =
            new Achievement("Mvp", "Embarcadero");

        david.Achievements.Mvp = first;
        david.Achievements.Mvp = equivalent;

        Assert.Single(david.Achievements);

        Assert.Same(first, david.Achievements[0]);
        Assert.NotSame(equivalent, david.Achievements[0]);
    }

    [Fact]
    public void SetMember_DoesNotReplaceEquivalentExistingAchievement()
    {
        var david = new Person();

        var existing =
            new Achievement("Mvp", "Embarcadero");

        david.Achievements.Add(existing);

        var replacement =
            new Achievement("Mvp", "Embarcadero");

        david.Achievements.Mvp = replacement;

        Assert.Single(david.Achievements);
        Assert.Same(existing, david.Achievements[0]);
    }

    [Fact]
    public void SetMember_AddsDistinctAchievement()
    {
        var david = new Person();

        var embarcadero =
            new Achievement("Mvp", "Embarcadero");

        var codeGear =
            new Achievement("Mvp", "CodeGear");

        david.Achievements.Mvp = embarcadero;
        david.Achievements.Mvp = codeGear;

        Assert.Equal(2, david.Achievements.Count);

        Assert.Same(embarcadero, david.Achievements[0]);
        Assert.Same(codeGear, david.Achievements[1]);
    }
}