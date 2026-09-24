namespace Dynamic.Tests.Fixtures;

public sealed class DeclarationScopeTests : RuntimeTestBase
{
    [Fact]
    public void Out_var_shadows_runtime_property_after_declaration()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public int Nickname { get; set; }
            """);

        bob.Nickname = 99;

        bob.ClassEval("""
            public int OutVarShadow()
            {
                int.TryParse("42", out var Nickname);
                return Nickname;
            }
            """);

        Assert.Equal(
            42,
            (int)bob.OutVarShadow());
    }

    [Fact]
    public void Foreach_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string ForeachShadow()
            {
                foreach (var Nickname in new[] { "A", "B" })
                {
                    return Nickname;
                }

                return "";
            }
            """);

        Assert.Equal(
            "A",
            (string)bob.ForeachShadow());
    }

    [Fact]
    public void Deconstruction_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string DeconstructionShadow()
            {
                var (Nickname, value) = ("Local Nick", 1);
                return Nickname;
            }
            """);

        Assert.Equal(
            "Local Nick",
            (string)bob.DeconstructionShadow());
    }

    [Fact]
    public void For_condition_pattern_variable_is_available_to_body()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string ForPattern()
            {
                object value = "For Nick";

                for (; value is string Nickname; )
                {
                    return Nickname;
                }

                return "";
            }
            """);

        Assert.Equal(
            "For Nick",
            (string)bob.ForPattern());
    }

    [Fact]
    public void While_condition_pattern_variable_is_available_to_body()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string WhilePattern()
            {
                object value = "While Nick";

                while (value is string Nickname)
                {
                    return Nickname;
                }

                return "";
            }
            """);

        Assert.Equal(
            "While Nick",
            (string)bob.WhilePattern());
    }

    [Fact]
    public void Do_condition_pattern_variable_shadows_receiver_in_condition()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public bool DoPattern()
            {
                object value = "Do Nick";
                var count = 0;

                do
                {
                    count++;
                }
                while (
                    count < 1 &&
                    value is string Nickname &&
                    Nickname.Length > 0);

                return count == 1;
            }
            """);

        Assert.True(
            (bool)bob.DoPattern());
    }
}
