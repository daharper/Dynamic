using Dynamic;
using Dynamic.Runtime;

// our two dynamic objects

dynamic alice = new Person { FirstName = "Alice" };
dynamic bob = new Person { FirstName = "Bob" };

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

foreach (var letter in bob.EchoThis("x", "y", "z"))
{
    Console.WriteLine(letter);
}

// generic eval

bob.Eval("""
         public T Echo<T>(T value)
         {
             return value;
         }
         """);

Console.WriteLine(bob.Echo("Hello, World!"));


bob.Eval("""
         public IEnumerable<(T, V)> GetPairs<T, V>(IEnumerable<T> keys, IEnumerable<V> values)
         {
             return keys.Zip(values, (k, v) => (k, v));
         }
         """);

var keys = new[] { "A", "B", "C" };
var values = new[] { 1, 2, 3 };

IEnumerable<(string, int)> pairs = bob.GetPairs(keys, values);

foreach (var (key, value) in pairs)
{
    Console.WriteLine($"{key}: {value}");
}

bob.Eval("""
         public T First<T>(IEnumerable<T> values)
         {
             return values.First();
         }
         """);

string first = bob.First(new List<string> { "A", "B", "C" });

Console.WriteLine(first); // A

bob.Eval("""
         public T Find<T>(
             IEnumerable<T> values,
             Func<T, bool> predicate)
         {
             return values.First(predicate);
         }            
         """);

var people = new[]
{
    new Person { FirstName = "Alice" },
    new Person { FirstName = "Bob" }
};

Func<Person, bool> predicate = p => p.FirstName == "Bob";

Console.WriteLine(bob.Find(people, predicate)); // Bob

bob.Eval("""
         public T Create<T>(Func<T> factory)
         {
             return factory();
         }
         """);

Func<string> factory = () => "Hello";

Console.WriteLine(bob.Create(factory));
