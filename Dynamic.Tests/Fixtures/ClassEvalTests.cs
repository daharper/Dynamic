namespace Dynamic.Tests.Fixtures;

public sealed class ClassEvalTests : RuntimeTestBase
{
    [Fact]
    public void ClassEval_method_can_read_CLR_instance_state()
    {
        dynamic alice = NewPerson("Alice");
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string DisplayName()
            {
                return FirstName;
            }
            """);

        Assert.Equal("Alice", (string)alice.DisplayName());
        Assert.Equal("Bob", (string)bob.DisplayName());
    }

    [Fact]
    public void Existing_instances_see_later_class_changes()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
            public string FirstVersion()
            {
                return FirstName;
            }
            """);

        Assert.Equal("Alice", (string)alice.FirstVersion());

        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string SecondVersion()
            {
                return FirstName + "!";
            }
            """);

        Assert.Equal("Alice!", (string)alice.SecondVersion());
        Assert.Equal("Bob!", (string)bob.SecondVersion());
    }

    [Fact]
    public void Runtime_method_can_call_runtime_method_from_same_patch()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string DisplayName()
            {
                return FirstName;
            }

            public string Greeting()
            {
                return "Hello " + DisplayName();
            }
            """);

        Assert.Equal("Hello Bob", (string)bob.Greeting());
    }

    [Fact]
    public void Runtime_auto_property_has_per_object_state()
    {
        dynamic alice = NewPerson("Alice");
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        alice.Nickname = "Al";
        bob.Nickname = "Robert";

        Assert.Equal("Al", (string)alice.Nickname);
        Assert.Equal("Robert", (string)bob.Nickname);
    }

    [Fact]
    public void Runtime_method_can_read_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string DisplayName()
            {
                return Nickname + " (" + FirstName + ")";
            }
            """);

        Assert.Equal("Robert (Bob)", (string)bob.DisplayName());
    }

    [Fact]
    public void Runtime_method_can_write_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string AddBang()
            {
                Nickname += "!";
                return Nickname;
            }
            """);

        Assert.Equal("Robert!", (string)bob.AddBang());
        Assert.Equal("Robert!", (string)bob.Nickname);
    }

    [Fact]
    public void Computed_runtime_property_can_read_CLR_state()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string UpperName
            {
                get
                {
                    return FirstName.ToUpperInvariant();
                }
            }
            """);

        Assert.Equal("BOB", (string)bob.UpperName);
    }
}
