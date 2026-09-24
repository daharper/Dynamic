using System.Collections.Frozen;

namespace Dynamic.Runtime;

/// <summary>
/// Represents the runtime class associated with an
/// <see cref="ActiveObject{TSelf}"/>, providing access to its
/// dynamically defined methods and properties.
/// </summary>
/// <typeparam name="TSelf">The concrete active object type.</typeparam>
public sealed class ActiveObjectClass<TSelf>
{
    private readonly FrozenDictionary<string, DynamicMethod<TSelf>> _methods;

    private readonly FrozenDictionary<string, DynamicProperty<TSelf>> _properties;

    public static ActiveObjectClass<TSelf> Empty { get; }
        = new(
            FrozenDictionary<string, DynamicMethod<TSelf>>.Empty,
            FrozenDictionary<string, DynamicProperty<TSelf>>.Empty);

    internal ActiveObjectClass(
        FrozenDictionary<string, DynamicMethod<TSelf>> methods,
        FrozenDictionary<string, DynamicProperty<TSelf>> properties)
    {
        _methods = methods;
        _properties = properties;
    }

    public bool TryGetMethod(string name, out DynamicMethod<TSelf> method)
        => _methods.TryGetValue(name, out method!);

    public bool TryGetProperty(string name, out DynamicProperty<TSelf> property)
        => _properties.TryGetValue(name, out property!);

    internal IEnumerable<string> MethodNames
        => _methods.Keys;

    internal IEnumerable<string> PropertyNames
        => _properties.Keys;

    internal ActiveObjectClassBuilder<TSelf> ToBuilder()
        => new(_methods, _properties);
}