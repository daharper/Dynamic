namespace Dynamic.Tests.Fixtures;

public sealed class LexicalShadowingTests : RuntimeTestBase
{
    [Fact]
    public void Parameter_shadows_runtime_property_and_CLR_member()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string Echo(string Nickname, string FirstName)
            {
                return Nickname + " / " + FirstName;
            }
            """);

        Assert.Equal(
            "Parameter Nick / Parameter First",
            (string)bob.Echo(
                "Parameter Nick",
                "Parameter First"));
    }

    [Fact]
    public void Local_variable_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string LocalShadow()
            {
                string Nickname = "Local Nick";
                return Nickname;
            }
            """);

        Assert.Equal("Local Nick", (string)bob.LocalShadow());
    }

    [Fact]
    public void Lambda_parameter_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string LambdaShadow()
            {
                Func<string, string> f =
                    Nickname => Nickname.ToUpperInvariant();

                return f("lambda");
            }
            """);

        Assert.Equal("LAMBDA", (string)bob.LambdaShadow());
    }

    [Fact]
    public void Local_function_parameter_shadows_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string LocalFunctionShadow()
            {
                string Inner(string Nickname)
                {
                    return Nickname;
                }

                return Inner("Local Function Nick");
            }
            """);

        Assert.Equal(
            "Local Function Nick",
            (string)bob.LocalFunctionShadow());
    }

    [Fact]
    public void Anonymous_method_parameter_shadows_CLR_member()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string AnonymousShadow()
            {
                Func<string, string> f =
                    delegate(string FirstName)
                    {
                        return FirstName;
                    };

                return f("Anonymous First");
            }
            """);

        Assert.Equal(
            "Anonymous First",
            (string)bob.AnonymousShadow());
    }

    [Fact]
    public void Catch_variable_can_shadow_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.ClassEval("""
            public string CatchShadow()
            {
                try
                {
                    throw new Exception("Caught Runtime");
                }
                catch (Exception Nickname)
                {
                    return Nickname.Message;
                }
            }
            """);

        Assert.Equal(
            "Caught Runtime",
            (string)bob.CatchShadow());
    }
}
