using Dynamic;

dynamic bob = new Person { FirstName = "Bob" };
dynamic alice = new Person { FirstName = "Alice" };

bob.Eval("""
              public string DisplayName()
              {
                  return FirstName;
              }

              public string Greeting()
              {
                  return "Hello " + DisplayName();
              }
              """);


Console.WriteLine(bob.Greeting());

// distinguish between long and int overloads
Console.WriteLine(bob.Send("Double", 42L));
Console.WriteLine(bob.Send("Double", 42));