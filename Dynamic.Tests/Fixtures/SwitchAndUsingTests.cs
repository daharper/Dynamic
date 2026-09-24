namespace Dynamic.Tests.Fixtures;

public sealed class SwitchAndUsingTests : RuntimeTestBase
{
    [Fact]
    public void Switch_statement_pattern_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string SwitchPattern()
            {
                object value = "Switch Nick";

                switch (value)
                {
                    case string Nickname:
                        return Nickname;

                    default:
                        return FirstName;
                }
            }
            """);

        Assert.Equal(
            "Switch Nick",
            (string)bob.SwitchPattern());
    }

    [Fact]
    public void Switch_expression_pattern_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string SwitchExpressionPattern()
            {
                object value = "Switch Nick";

                return value switch
                {
                    string Nickname => Nickname,
                    _ => FirstName
                };
            }
            """);

        Assert.Equal(
            "Switch Nick",
            (string)bob.SwitchExpressionPattern());
    }

    [Fact]
    public void Classic_using_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public IDisposable Nickname { get; set; }");

        bob.ClassEval("""
            public bool UsingShadow()
            {
                using (
                    var Nickname =
                        new MemoryStream())
                {
                    return Nickname.CanRead;
                }
            }
            """);

        Assert.True(
            (bool)bob.UsingShadow());
    }
}
