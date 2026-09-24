using System.Reflection;
using Dynamic.Tests.Mocks;
using Microsoft.CSharp.RuntimeBinder;

namespace Dynamic.Tests.Fixtures;

public sealed class InspectionTests : RuntimeTestBase
{
    [Fact]
    public void Inspection_distinguishes_object_class_and_clr_members()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string DisplayName()
                        {
                            return FirstName;
                        }
                        """);

        alice.Eval("""
                   public string SecretName()
                   {
                       return "Secret";
                   }
                   """);

        var objectMethods = ((Person)alice).ObjectMethods();

        var classMethods = ((Person)alice).ClassMethods();

        var methods = ((Person)alice).Methods();

        var properties = ((Person)alice).Properties();

        Assert.Contains("SecretName", objectMethods);

        Assert.DoesNotContain("DisplayName", objectMethods);

        Assert.Contains("DisplayName", classMethods);

        Assert.DoesNotContain("SecretName", classMethods);

        Assert.Contains("SecretName", methods);

        Assert.Contains("DisplayName", methods);

        Assert.Contains("ToString", methods);

        Assert.Contains("FirstName", properties);
    }

    [Fact]
    public void Send_invokes_runtime_method_by_name()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string DisplayName()
                        {
                            return FirstName;
                        }
                        """);

        var result = alice.Send("DisplayName");

        Assert.Equal("Alice", (string)result);
    }

    [Fact]
    public void Send_passes_arguments_to_runtime_method()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string Join(
                            string left,
                            string right)
                        {
                            return $"{left}:{right}";
                        }
                        """);

        var result = alice.Send("Join", "A", "B");

        Assert.Equal("A:B", (string)result);
    }

    [Fact]
    public void Send_routes_missing_method_to_method_missing()
    {
        dynamic alice =
            NewPerson("Alice");

        var result =
            alice.Send(
                "DoesNotExist");

        Assert.Equal(
            "Missing: DoesNotExist",
            (string)result);
    }

    [Fact]
    public void Send_prefers_singleton_method_over_instance_method()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string Greeting()
                        {
                            return "Class";
                        }
                        """);

        alice.Eval("""
                   public string Greeting()
                   {
                       return "Singleton";
                   }
                   """);

        Assert.Equal("Singleton", (string)alice.Send("Greeting"));

        dynamic bob = NewPerson("Bob");

        Assert.Equal("Class", (string)bob.Send("Greeting"));
    }

    [Fact]
    public void Send_of_T_returns_typed_result()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string DisplayName()
                        {
                            return FirstName;
                        }
                        """);

        var result = alice.Send<string>("DisplayName");

        Assert.Equal("Alice", result);
    }

    [Fact]
    public void Send_of_T_passes_arguments()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public int Add(
                            int left,
                            int right)
                        {
                            return left + right;
                        }
                        """);

        var result = alice.Send<int>("Add", 10, 20);

        Assert.Equal(30, result);
    }

    [Fact]
    public void Send_supports_parenthesized_command_syntax()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public int Add(int left, int right)
                        {
                            return left + right;
                        }
                        """);

        Assert.Equal(30, (int)alice.Send("Add(10, 20)"));
    }

    [Fact]
    public void Send_supports_whitespace_command_syntax()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public int Add(int left, int right)
                        {
                            return left + right;
                        }
                        """);

        Assert.Equal(42, (int)alice.Send("Add 30 12"));
    }

    [Fact]
    public void Send_preserves_spaces_inside_quoted_strings()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string Say(string value)
                        {
                            return value;
                        }
                        """);

        Assert.Equal("Hello World", (string)alice.Send("""Say "Hello World" """));
    }

    [Fact]
    public void Send_supports_direct_name_and_argument_dispatch()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string Join(
                            string left,
                            string right)
                        {
                            return $"{left}:{right}";
                        }
                        """);

        Assert.Equal("A:B", (string)alice.Send("Join", "A", "B"));
    }

    [Fact]
    public void Send_uses_default_method_missing_behavior()
    {
        dynamic item = new DefaultMissingObject();

        var exception =
            Assert.Throws<MissingMethodException>(
                () =>
                {
                    item.Send("DoesNotExist");
                });

        Assert.Equal("Method 'DoesNotExist' was not found on 'DefaultMissingObject'.", exception.Message);
    }

    [Fact]
    public void Send_prefers_object_method_over_class_and_clr_methods()
    {
        dynamic alice =
            NewPerson("Alice");

        alice.ClassEval("""
                        public string ToString()
                        {
                            return "Class";
                        }
                        """);

        alice.Eval("""
                   public string ToString()
                   {
                       return "Object";
                   }
                   """);

        var result =
            alice.Send("ToString");

        Assert.Equal(
            "Object",
            (string)result);
    }

    [Fact]
    public void Send_prefers_class_method_over_clr_method()
    {
        dynamic alice =
            NewPerson("Alice");

        alice.ClassEval("""
                        public string ToString()
                        {
                            return "Class";
                        }
                        """);

        var result =
            alice.Send("ToString");

        Assert.Equal(
            "Class",
            (string)result);
    }

    [Fact]
    public void Send_falls_back_to_clr_method()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("ToString");

        Assert.Equal("Dynamic.Tests.Mocks.Person", (string)result);
    }

    [Fact]
    public void Send_routes_to_method_missing_when_no_method_matches()
    {
        dynamic alice =
            NewPerson("Alice");

        var result =
            alice.Send("DoesNotExist");

        Assert.Equal(
            "Missing: DoesNotExist",
            (string)result);
    }

    [Fact]
    public void Send_prefers_exact_numeric_clr_overload()
    {
        dynamic bob = NewPerson("Bob");

        var longResult = bob.Send("Double", 42L);

        var intResult = bob.Send("Double", 42);

        Assert.IsType<long>(longResult);

        Assert.Equal(84L, (long)longResult);

        Assert.IsType<int>(intResult);
        Assert.Equal(84, (int)intResult);
    }

    [Fact]
    public void Send_converts_numeric_argument_for_clr_method()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Double", 21);

        Assert.Equal(42L, (long)result);
    }

    [Fact]
    public void HasMethod_checks_object_class_and_clr_methods()
    {
        dynamic alice =
            NewPerson("Alice");

        alice.ClassEval("""
                        public string DisplayName()
                        {
                            return FirstName;
                        }
                        """);

        alice.Eval("""
                   public string SecretName()
                   {
                       return "Secret";
                   }
                   """);

        Assert.True(alice.HasMethod("SecretName"));

        Assert.True(alice.HasMethod("DisplayName"));

        Assert.True(alice.HasMethod("ToString"));

        Assert.False(alice.HasMethod("DoesNotExist"));
    }

    [Fact]
    public void Send_throws_when_clr_overloads_have_equal_match_quality()
    {
        dynamic alice = NewPerson("Alice");

        var exception = 
            Assert.Throws<AmbiguousMatchException>(
                () =>
                {
                    alice.Send("Choose", 10, 20);
                });

        Assert.Contains("Choose", exception.Message);
    }

    [Fact]
    public void Send_does_not_apply_narrowing_numeric_conversion_for_clr_method()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("AcceptInt", 42L);

        Assert.Equal("Missing: AcceptInt", (string)result);
    }

    [Fact]
    public void Send_prefers_more_specific_reference_type_for_null_argument()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Describe", (object?)null);

        Assert.Equal("string", (string)result);
    }

    [Fact]
    public void Send_treats_null_as_single_null_argument()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Describe", null);

        Assert.Equal("string", (string)result);
    }

    [Fact]
    public void Send_uses_default_value_for_optional_clr_parameter()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Greet", "Bob");

        Assert.Equal("Hello Bob!", (string)result);
    }

    [Fact]
    public void Send_packs_extra_arguments_into_clr_params_array()
    {
        dynamic alice =
            NewPerson("Alice");

        var result =
            alice.Send("Sum", 10, 20, 30);

        Assert.Equal(60, (int)result);
    }
    
    [Fact]
    public void Send_supplies_empty_array_for_clr_params_parameter()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Sum");

        Assert.Equal(0, (int)result);
    }
    
    [Fact]
    public void Send_accepts_prepacked_array_for_clr_params_parameter()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Sum", new[] { 10, 20, 30 });

        Assert.Equal(60, (int)result);
    }
    
    [Fact]
    public void Send_binds_fixed_and_params_clr_parameters()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Join", ", ", "A", "B", "C");

        Assert.Equal("A, B, C", (string)result);
    }
    
    [Fact]
    public void Send_rejects_incompatible_clr_params_argument()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("Sum", 10, "20", 30);

        Assert.Equal("Missing: Sum", (string)result);
    }
    
    [Fact]
    public void Send_unwraps_clr_target_invocation_exception()
    {
        dynamic alice = NewPerson("Alice");

        var exception = Assert.Throws<InvalidOperationException>(() => alice.Send("Explode"));

        Assert.Equal("Boom", exception.Message);
    }
    
    [Fact]
    public void Send_binds_non_null_value_to_nullable_clr_parameter()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("DescribeNullable", 42);

        Assert.Equal("42", (string)result);
    }
    
    [Fact]
    public void Send_binds_null_to_nullable_clr_parameter()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("DescribeNullable", (object?)null);

        Assert.Equal("null", (string)result);
    }
    
    [Fact]
    public void Send_prefers_more_specific_reference_type_overload()
    {
        dynamic alice = NewPerson("Alice");

        var result = alice.Send("ChooseReference", new MemoryStream());

        Assert.Equal("stream", (string)result);
    }
    
    [Fact]
    public void HasProperty_checks_object_class_and_clr_properties()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("public string Nickname { get; set; }");

        alice.Eval("""
                   public string SecretName
                   {
                       get
                       {
                           return "Secret";
                       }
                   }
                   """);

        Assert.True(alice.HasProperty("SecretName"));

        Assert.True(alice.HasProperty("Nickname"));

        Assert.True(alice.HasProperty("FirstName"));

        Assert.False(alice.HasProperty("DoesNotExist"));
    }

    [Fact]
    public void Object_property_shadows_class_property_even_without_setter()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("public string Nickname { get; set; }");

        alice.Nickname = "Class value";

        alice.Eval("""
                   public string Nickname
                   {
                       get
                       {
                           return "Object value";
                       }
                   }
                   """);

        Assert.Equal("Object value", (string)alice.Nickname);

        Assert.Throws<RuntimeBinderException>(() => { alice.Nickname = "Should fail"; });
    }

    [Fact]
    public void Class_property_without_setter_does_not_materialize_object_property()
    {
        dynamic alice = NewPerson("Alice");

        alice.ClassEval("""
                        public string DisplayName
                        {
                            get
                            {
                                return FirstName;
                            }
                        }
                        """);

        Assert.Throws<RuntimeBinderException>(() => { alice.DisplayName = "Bob"; });

        Assert.DoesNotContain("DisplayName", ((Person)alice).ObjectProperties());
    }

    [Fact]
    public void Dynamic_invocation_of_missing_method_uses_normal_dlr_failure()
    {
        dynamic alice =
            NewPerson("Alice");

        Assert.Throws<RuntimeBinderException>(
            () =>
            {
                _ = alice.DoesNotExist();
            });
    }

    [Fact]
    public void Default_method_missing_throws_missing_method_exception()
    {
        dynamic item = new DefaultMissingObject();

        var exception =
            Assert.Throws<MissingMethodException>(
                () =>
                {
                    item.Send("DoesNotExist");
                });

        Assert.Contains("DoesNotExist", exception.Message);
    }
}
