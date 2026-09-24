using Dynamic;

dynamic bob = new Person();

Console.WriteLine(bob.Send("Double", 42L));
Console.WriteLine(bob.Send("Double", 42));