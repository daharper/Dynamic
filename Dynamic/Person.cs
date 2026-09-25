using Dynamic.Runtime;

namespace Dynamic;

public sealed class Person : ActiveObject<Person>
{
    public string FirstName { get; set; } = "";

    public long Double(long value)
    {
        return value * 2;
    }

    public int Double(int value)
    {
        return value * 2;
    }

    public int AcceptInt(int value)
    {
        return value;
    }

    public int Sum(params int[] values)
    {
        return values.Sum();
    }
}

public record class Cat(string Name);

public record class Dog(string Name);

public record class Bird(string Name);

public union Pet(Cat, Dog, Bird);