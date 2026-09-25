using Dynamic;
using Dynamic.Runtime;

dynamic bob = new Person { FirstName = "Bob" };
dynamic alice = new Person { FirstName = "Alice" };

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

Console.WriteLine(bob.Greeting());          // Hello Bob
Console.WriteLine(alice.Greeting());        // Hello Alice

alice.Eval("""
           public string Greeting()
           {
               return "Bonjour " + DisplayName();
           }
           """);

Console.WriteLine(bob.Greeting());           // Hello Bob
Console.WriteLine(alice.Greeting());         // Bonjour Alice

Console.WriteLine(bob.Send("Double", 42L));  // calls the long overload
Console.WriteLine(bob.Send("Double", 42));   // calls the int overload

Console.WriteLine(Eval.Run<int>("18 + 24")); // 42