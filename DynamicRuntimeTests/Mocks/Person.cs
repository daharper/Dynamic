using Dynamic.Runtime;

namespace Dynamic.Tests.Mocks;


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

    public string Choose(int x, long y)
    {
        return "int-long";
    }

    public string Choose(long x, int y)
    {
        return "long-int";
    }

    public string Join(string separator, params string[] values)
    {
        return string.Join(separator, values);
    }
    
    public string DescribeNullable(int? value)
    {
        return value?.ToString() ?? "null";
    }
    
    public void Explode()
    {
        throw new InvalidOperationException("Boom");
    }
    
    public int Sum(params int[] values)
    {
        return values.Sum();
    }
    
    public string ChooseReference(object value)
    {
        return "object";
    }

    public string ChooseReference(Stream value)
    {
        return "stream";
    }
    
    public string Describe(string? value)
    {
        return "string";
    }

    public string Describe(object? value)
    {
        return "object";
    }
    
    public string Greet(string name, string punctuation = "!")
    {
        return $"Hello {name}{punctuation}";
    }

    protected override object? MethodMissing(RuntimeMessage message)
    {
        return $"Missing: {message.Name}";
    }
}