using Dynamic.Runtime;

namespace Dynamic.Tests.Fixtures;

public sealed class PatternScopeTests : RuntimeTestBase
{
    [Fact]
    public void If_pattern_variable_shadows_runtime_property_inside_true_branch()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public string PatternShadow()
            {
                object value = "Pattern Nick";

                if (value is string Nickname)
                {
                    return Nickname;
                }

                return "";
            }
            """);

        Assert.Equal(
            "Pattern Nick",
            (string)bob.PatternShadow());
    }

    [Fact]
    public void Pattern_variable_after_if_is_preserved_for_definite_assignment()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        var exception =
            Assert.Throws<RuntimeCompilationException>(
                () =>
                {
                    bob.ClassEval("""
                                  public string PatternAfterIfInvalid()
                                  {
                                      object value = 123;

                                      if (value is string Nickname)
                                      {
                                      }

                                      return Nickname;
                                  }
                                  """);
                });

        Assert.Contains("CS0165",exception.Message);
    }

    [Fact]
    public void Pattern_variable_in_later_else_if_is_not_rewritten_as_runtime_property()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        var exception =
            Assert.Throws<RuntimeCompilationException>(() =>
            {
                bob.ClassEval("""
                              public string PatternLeakAcrossElseIf()
                              {
                                  object value = 123;

                                  if (value is string Nickname)
                                  {
                                      return Nickname;
                                  }
                                  else if (Nickname.Length > 0)
                                  {
                                      return "Unexpected";
                                  }

                                  return FirstName;
                              }
                              """);
            });

        Assert.Contains("CS0165", exception.Message);
    }

    [Fact]
    public void Pattern_variable_after_while_is_out_of_scope()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        var exception =
            Assert.Throws<RuntimeCompilationException>(() =>
            {
                bob.ClassEval("""
                              public string PatternAfterWhileInvalid()
                              {
                                  object value = 123;

                                  while (value is string Nickname)
                                  {
                                      return Nickname;
                                  }

                                  return Nickname;
                              }
                              """);
            });

        Assert.Contains("CS0103", exception.Message);
    }

    [Fact]
    public void Negated_pattern_variable_is_available_on_false_branch()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string NegatedPattern()
            {
                object value = "Pattern Nick";

                if (value is not string Nickname)
                {
                    return "";
                }

                return Nickname;
            }
            """);

        Assert.Equal(
            "Pattern Nick",
            (string)bob.NegatedPattern());
    }

    [Fact]
    public void Else_if_pattern_chain_keeps_each_pattern_binding_correct()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string PatternElseIfChain()
            {
                object value = "Second";

                if (value is int Number)
                {
                    return Number.ToString();
                }
                else if (value is string Nickname)
                {
                    return Nickname;
                }
                else
                {
                    return FirstName;
                }
            }
            """);

        Assert.Equal(
            "Second",
            (string)bob.PatternElseIfChain());
    }

    [Fact]
    public void Conditional_expression_pattern_uses_local_on_assigned_branch()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string ConditionalPattern()
            {
                object value = "Conditional Nick";

                return value is string Nickname
                    ? Nickname
                    : FirstName;
            }
            """);

        Assert.Equal(
            "Conditional Nick",
            (string)bob.ConditionalPattern());
    }

    [Fact]
    public void Logical_and_pattern_variable_shadows_runtime_property_on_rhs()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
            public string Nickname { get; set; }
            """);

        bob.Nickname = "Robert";

        bob.ClassEval("""
            public bool LogicalAndPattern()
            {
                object value = "Pattern Nick";

                return
                    value is string Nickname &&
                    Nickname.Length > 0;
            }
            """);

        Assert.True(
            (bool)bob.LogicalAndPattern());
    }

    [Fact]
    public void Invalid_logical_or_pattern_use_remains_a_compiler_error()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        var exception = Assert.Throws<RuntimeCompilationException>(() =>
                {
                    bob.ClassEval("""
                                  public bool InvalidOrPatternUse()
                                  {
                                      object value = 123;

                                      return
                                          value is string Nickname ||
                                          Nickname.Length > 0;
                                  }
                                  """);
                });

        Assert.Contains("CS0165", exception.Message);
    }

    [Fact]
    public void Pattern_variable_after_for_is_out_of_scope()
    {
        dynamic bob = NewPerson("Bob");

        var exception =
            Assert.Throws<RuntimeCompilationException>(
                () =>
                {
                    bob.ClassEval("""
                                  public string PatternAfterForInvalid()
                                  {
                                      object value = 123;

                                      for (; value is string Nickname;)
                                      {
                                          return Nickname;
                                      }

                                      return Nickname;
                                  }
                                  """);
                });

        Assert.Contains("CS0103", exception.Message);
    }

    [Fact]
    public void Pattern_variable_from_for_initializer_is_in_scope_but_not_definitely_assigned()
    {
        dynamic bob = NewPerson("Bob");

        var exception =
            Assert.Throws<RuntimeCompilationException>(
                () =>
                {
                    bob.ClassEval("""
                                  public string PatternInForInitializer()
                                  {
                                      object value = "Pattern Nick";

                                      for (
                                          bool matched = value is string Nickname;
                                          matched;
                                          matched = false)
                                      {
                                          return Nickname;
                                      }

                                      return FirstName;
                                  }
                                  """);
                });

        Assert.Contains("CS0165", exception.Message);
    }

    [Fact]
    public void Pattern_variable_in_for_condition_is_available_in_body()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("""
                      public string PatternDirectlyInForCondition()
                      {
                          object value = "Pattern Nick";

                          for (; value is string Nickname;)
                          {
                              return Nickname;
                          }

                          return FirstName;
                      }
                      """);

        Assert.Equal(
            "Pattern Nick",
            (string)bob.PatternDirectlyInForCondition());
    }

    [Fact]
    public void Pattern_variable_in_for_iterator_does_not_leak_after_for()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
                      public string PatternInForIterator()
                      {
                          object value = "Pattern Nick";
                          int count = 0;

                          for (
                              ;
                              count < 1;
                              count +=
                                  value is string Nickname &&
                                  Nickname.Length > 0
                                      ? 1
                                      : 1)
                          {
                          }

                          return Nickname;
                      }
                      """);

        Assert.Equal("Robert", (string)bob.PatternInForIterator());
    }

    [Fact]
    public void Pattern_variable_in_using_declaration_shadows_runtime_property_for_rest_of_block()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        var exception = Assert.Throws<RuntimeCompilationException>(() =>
                {
                    bob.ClassEval("""
                                  public string PatternInUsingDeclaration()
                                  {
                                      object value = "Pattern Nick";

                                      using var stream =
                                          value is string Nickname
                                              ? new System.IO.MemoryStream()
                                              : new System.IO.MemoryStream();

                                      return Nickname;
                                  }
                                  """);
                });

        Assert.Contains("CS0165", exception.Message);
    }

    [Fact]
    public void Pattern_variable_in_nested_using_declaration_does_not_leak_outside_block()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
                      public string PatternInNestedUsingDeclaration()
                      {
                          object value = "Pattern Nick";

                          {
                              using var stream =
                                  value is string Nickname
                                      ? new System.IO.MemoryStream()
                                      : new System.IO.MemoryStream();
                          }

                          return Nickname;
                      }
                      """);

        Assert.Equal("Robert", (string)bob.PatternInNestedUsingDeclaration());
    }

    [Fact]
    public void Pattern_variable_in_lock_expression_does_not_leak_after_lock()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
                      public string PatternInLockExpression()
                      {
                          object value = "Pattern Nick";

                          lock (
                              value is string Nickname
                                  ? new object()
                                  : new object())
                          {
                              var inside =
                                  value is string InnerNickname
                                      ? InnerNickname
                                      : "";
                          }

                          return Nickname;
                      }
                      """);

        Assert.Equal("Robert", (string)bob.PatternInLockExpression());
    }

    [Fact]
    public void Fixed_variable_shadows_runtime_property_only_within_fixed_statement()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
                      public unsafe string FixedVariableShadowing()
                      {
                          char[] chars = ['A', 'B'];

                          fixed (char* Nickname = chars)
                          {
                              if (*Nickname != 'A')
                              {
                                  return "Wrong";
                              }
                          }

                          return Nickname;
                      }
                      """);

        Assert.Equal("Robert", (string)bob.FixedVariableShadowing());
    }

    [Fact]
    public void Switch_expression_pattern_variable_shadows_runtime_property_only_in_its_arm()
    {
        dynamic bob = NewPerson("Bob");

        bob.ClassEval("public string Nickname { get; set; }");

        bob.Nickname = "Robert";

        bob.ClassEval("""
                      public string ModernSwitch(object value)
                      {
                          return value switch
                          {
                              string Nickname => Nickname,
                              _ => Nickname
                          };
                      }
                      """);

        Assert.Equal("Pattern Nick", (string)bob.ModernSwitch("Pattern Nick"));

        Assert.Equal("Robert", (string)bob.ModernSwitch(42));
    }
}