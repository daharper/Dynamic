using Dynamic;
using Dynamic.Runtime;

// our two dynamic objects

dynamic bob = new Person { FirstName = "Bob" };
dynamic alice = new Person { FirstName = "Alice" };

// validating object method precedence over class method

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

Console.WriteLine(bob.Greeting());              // Hello Bob
Console.WriteLine(alice.Greeting());            // Hello Alice

alice.Eval("""
           public string Greeting()
           {
               return "Bonjour " + DisplayName();
           }
           """);

Console.WriteLine(bob.Greeting());               // Hello Bob
Console.WriteLine(alice.Greeting());             // Bonjour Alice

// overload and params resolution

Console.WriteLine(bob.Send("Double", 42L));      // calls the long overload
Console.WriteLine(bob.Send("Double", 42));       // calls the int overload
Console.WriteLine(bob.Send("Sum", 1, 2, 3, 4));  // handles params

// simple eval

Console.WriteLine(Eval.Run<int>("18 + 24"));     // 42

// lexical scope

bob.ClassEval("""
              public string Nickname { get; set; }

              public string ScopeExample()
              {
                  var query =
                      from Nickname in new[] { "ONE", "TWO" }
                      select Nickname;

                  return string.Join(", ", query);
              }
              """);

bob.Nickname = "Robert";

Console.WriteLine(bob.ScopeExample());           // ONE, TWO
Console.WriteLine(bob.Nickname);                 // Robert