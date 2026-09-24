namespace Dynamic.Tests.Fixtures;

public sealed class LinqScopeTests : RuntimeTestBase
{
    [Fact]
    public void Linq_from_range_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string LinqFromShadow()
            {
                var query =
                    from Nickname in new[] { "ONE", "TWO" }
                    select Nickname;

                return string.Join(", ", query);
            }
            """);

        Assert.Equal(
            "ONE, TWO",
            (string)bob.LinqFromShadow());
    }

    [Fact]
    public void Linq_let_range_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string LinqLetShadow()
            {
                var query =
                    from value in new[] { "abc" }
                    let Nickname = value.ToUpperInvariant()
                    select Nickname;

                return query.Single();
            }
            """);

        Assert.Equal(
            "ABC",
            (string)bob.LinqLetShadow());
    }

    [Fact]
    public void Linq_query_continuation_has_its_own_range_variable()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string LinqContinuation()
            {
                var query =
                    from value in new[] { "a", "b" }
                    select value.ToUpperInvariant()
                    into Nickname
                    select Nickname;

                return string.Join("", query);
            }
            """);

        Assert.Equal(
            "AB",
            (string)bob.LinqContinuation());
    }
}
