using System.Linq.Expressions;
using Dynamic.Runtime;
using Dynamic.Tests.Mocks;
using Microsoft.CSharp.RuntimeBinder;

namespace Dynamic.Tests.Fixtures;

public sealed class EvalTests : RuntimeTestBase
{
    [Fact]
    public void Eval_adds_method_to_one_instance_only()
    {
        dynamic alice = NewPerson("Alice");
        dynamic bob = NewPerson("Bob");

        alice.Eval("""
            public string Secret()
            {
                return FirstName + " only";
            }
            """);

        Assert.Equal("Alice only", (string)alice.Secret());

        Assert.Throws<RuntimeBinderException>(() => { _ = bob.Secret(); });
    }

    [Fact]
    public void Instance_member_takes_precedence_over_class_member()
    {
        dynamic alice = NewPerson("Alice");
        dynamic bob = NewPerson("Bob");

        alice.ClassEval("""
                        public string Label()
                        {
                            return "class";
                        }
                        """);

        // First prove that the class member exists
        // before we install an instance override.
        Assert.Equal("class", (string)alice.Label());
        Assert.Equal("class", (string)bob.Label());

        alice.Eval("""
                   public string Label()
                   {
                       return "instance";
                   }
                   """);

        // Alice should now hit the instance overlay.
        Assert.Equal("instance", (string)alice.Label());

        // Bob should still hit the shared class definition.
        Assert.Equal("class", (string)bob.Label());
    }

    [Fact]
    public void Eval_auto_property_state_is_object_specific()
    {
        dynamic alice = NewPerson("Alice");
        dynamic bob = NewPerson("Bob");

        alice.Eval("public string PrivateNickname { get; set; }");

        alice.PrivateNickname = "Al";

        Assert.Equal("Al", (string)alice.PrivateNickname);
        Assert.False(bob.HasProperty("PrivateNickname"));
        Assert.Null(bob.PrivateNickname);
        Assert.True(bob.HasProperty("PrivateNickname"));
        Assert.Null(bob.PrivateNickname);
        Assert.Equal("Al", (string)alice.PrivateNickname);
    }

    [Fact]
    public void Frozen_object_does_not_create_missing_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.Freeze = FreezeMode.Partial;

        Assert.Throws<RuntimeBinderException>(() => { _ = bob.PrivateNickname; });
        Assert.False(bob.HasProperty("PrivateNickname"));
    }

    [Fact]
    public void Freeze_modes_control_runtime_property_mutation()
    {
        dynamic alice = NewPerson("Alice");

        alice.Nickname = "Al";

        Assert.Equal("Al", (string)alice.Nickname);

        alice.Freeze = FreezeMode.Partial;

        alice.Nickname = "Ali";

        Assert.Equal("Ali", (string)alice.Nickname);

        alice.Freeze = FreezeMode.Fully;

        Assert.Equal("Ali", (string)alice.Nickname);
        Assert.Throws<RuntimeBinderException>(() => { alice.Nickname = "Alice"; });
        Assert.Equal("Ali", (string)alice.Nickname);
    }

    [Fact]
    public void Run_evaluates_expression()
    {
        var result = Eval.Run("1 + 1");

        Assert.Equal(2, result);
    }

    [Fact]
    public void Run_of_T_returns_typed_result()
    {
        var result = Eval.Run<int>("40 + 2");

        Assert.Equal(42, result);
    }

    [Fact]
    public void Run_of_T_evaluates_statement_block()
    {
        var result = 
            Eval.Run<int>(
                """
                var x = 10;
                var y = 20;

                return x + y;
                """);

        Assert.Equal(30, result);
    }

    [Fact]
    public void Run_supports_normal_CSharp_code()
    {
        var result =
            Eval.Run<string>(
                """
                var values =
                    new List<int>
                    {
                        10,
                        20,
                        30
                    };

                return string.Join(
                    ",",
                    values.Select(
                        value => value * 2));
                """);

        Assert.Equal("20,40,60", result);
    }

    [Fact]
    public void Fully_frozen_object_rejects_eval()
    {
        dynamic alice = NewPerson("Alice");

        alice.Freeze = FreezeMode.Fully;

        Assert.Throws<InvalidOperationException>(
            () =>
            {
                alice.Eval("""
                           public string Secret()
                           {
                               return "Secret";
                           }
                           """);
            });
    }

    [Fact]
    public void Fully_frozen_object_cannot_write_active_class_property()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("public string Nickname { get; set; }");

        alice.Nickname = "Al";

        Assert.Equal("Al", (string)alice.Nickname);

        alice.Freeze = FreezeMode.Fully;
        
        Assert.Equal("Al", (string)alice.Nickname);
        Assert.Throws<RuntimeBinderException>(() => { alice.Nickname = "Alice"; });
        Assert.Equal("Al", (string)alice.Nickname);
    }

    [Fact]
    public void Partially_frozen_object_allows_eval()
    {
        dynamic alice = NewPerson("Alice");

        alice.Freeze = FreezeMode.Partial;

        alice.Eval("""
                   public string Secret()
                   {
                       return "Secret";
                   }
                   """);

        Assert.Equal("Secret", (string)alice.Secret());
    }

    [Fact]
    public void Eval_can_define_and_invoke_generic_method()
    {
        dynamic bob = NewPerson("Bob");

        bob.Eval("""
                 public T Echo<T>(T value)
                 {
                     return value;
                 }
                 """);

        string result = bob.Echo("Hello");

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Eval_can_infer_generic_arguments_from_generic_interfaces()
    {
        dynamic bob = NewPerson("Bob");

        bob.Eval("""
                 public IEnumerable<(T, V)> GetPairs<T, V>(
                     IEnumerable<T> keys,
                     IEnumerable<V> values)
                 {
                     return keys.Zip(values, (k, v) => (k, v));
                 }
                 """);

        var keys = new[] { "A", "B", "C" };
        var values = new[] { 1, 2, 3 };

        IEnumerable<(string, int)> result =
            bob.GetPairs(keys, values);

        Assert.Equal(
            new[]
            {
                ("A", 1),
                ("B", 2),
                ("C", 3)
            },
            result);
    }

    [Fact]
    public void Eval_can_infer_generic_argument_from_constructed_generic_type()
    {
        dynamic bob = NewPerson("Bob");

        bob.Eval("""
                 public T Create<T>(Func<T> factory)
                 {
                     return factory();
                 }
                 """);

        Func<string> factory = () => "Hello";

        string result = bob.Create(factory);

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Eval_rejects_conflicting_generic_argument_inferences()
    {
        dynamic bob = NewPerson("Bob");

        bob.Eval("""
                 public T Choose<T>(T first, T second)
                 {
                     return first;
                 }
                 """);

        var exception = Assert.Throws<InvalidOperationException>(() => bob.Choose("A", 42));

        Assert.Contains("Conflicting inferences for generic parameter 'T'", exception.Message);
    }

    [Fact]
    public void Eval_can_return_expression_tree()
    {
        ActiveRuntime.Register(typeof(Person));

        dynamic bob = new Person
        {
            FirstName = "Bob"
        };

        bob.Eval("""
                 public Expression<Func<Person, bool>> FirstNameIs(string name)
                 {
                     return person => person.FirstName == name;
                 }
                 """);

        Expression<Func<Person, bool>> specification = bob.FirstNameIs("Bob");

        Assert.Equal(ExpressionType.Equal, specification.Body.NodeType);

        var predicate = specification.Compile();

        Assert.True(predicate(bob));
    }

    [Fact]
    public void Runtime_method_takes_precedence_over_clr_method()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string ToString()
                        {
                            return "Runtime";
                        }
                        """);

        Assert.Equal("Runtime", (string)alice.Send("ToString"));
    }

}
