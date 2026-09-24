using System.Reflection;

namespace Dynamic.Runtime;

public delegate object? DynamicMethod<in T>(T self, object?[] args);

public sealed class DynamicProperty<T>
{
    public Func<T, object?>? Getter { get; init; }

    public Action<T, object?>? Setter { get; init; }
}

public sealed record RuntimeMessage(string Name, object?[] Arguments);

internal enum RuntimeArgumentMatch
{
    None,
    Converted,
    Exact
}

public enum FreezeMode
{
    None,
    Partial,
    Fully
}

internal readonly record struct RuntimeArgumentBinding(object?[] Arguments, int Score);

internal readonly record struct ClrMethodCandidate(MethodInfo Method, RuntimeArgumentBinding Binding);

internal readonly record struct RuntimeMethodBinding(MethodInfo Method, object?[] Arguments);