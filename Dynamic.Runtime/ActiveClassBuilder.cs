using System.Collections.Frozen;

namespace Dynamic.Runtime;

/// <summary>
/// Builds an <see cref="ActiveClass{TSelf}"/> by collecting the
/// dynamically defined methods and properties for the class.
/// </summary>
/// <typeparam name="TSelf"> The concrete active object type.</typeparam>
public sealed class ActiveClassBuilder<TSelf>
{
    private readonly Dictionary<string, DynamicMethod<TSelf>> _methods;

    private readonly Dictionary<string, DynamicProperty<TSelf>> _properties;

    internal ActiveClassBuilder(
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

    internal ActiveClass<TSelf> Build()
        => new(_methods.ToFrozenDictionary(), _properties.ToFrozenDictionary());
}