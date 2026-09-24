using System.Collections.Frozen;

namespace Dynamic.Runtime;

/// <summary>
/// Builds the runtime class for an <see cref="ActiveObject{TSelf}"/> by
/// collecting its dynamically defined methods and properties.
/// </summary>
/// <typeparam name="TSelf"> The concrete active object type.</typeparam>
public sealed class ActiveObjectClassBuilder<TSelf>
{
    private readonly Dictionary<string, DynamicMethod<TSelf>> _methods;

    private readonly Dictionary<string, DynamicProperty<TSelf>> _properties;

    internal ActiveObjectClassBuilder(
        IReadOnlyDictionary<string, DynamicMethod<TSelf>> methods,
        IReadOnlyDictionary<string, DynamicProperty<TSelf>> properties)
    {
        _methods = new(methods);
        _properties = new(properties);
    }

    public void Method(string name, DynamicMethod<TSelf> method)
        => _methods[name] = method;

    public void Property(string name, DynamicProperty<TSelf> property)
        => _properties[name] = property;

    internal ActiveObjectClass<TSelf> Build()
        => new(_methods.ToFrozenDictionary(), _properties.ToFrozenDictionary());
}