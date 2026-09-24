namespace Dynamic.Runtime;

/// <summary>
/// Provides a registry for the active classes associated with a given object type.
/// </summary>
/// <typeparam name="TSelf">
/// The active object type whose classes are registered.
/// </typeparam>
public static class ActiveClassRegistry<TSelf>
{
    private static ActiveClass<TSelf> _current
        = ActiveClass<TSelf>.Empty;

    public static ActiveClass<TSelf> Current
        => Volatile.Read(ref _current);

    internal static void Replace(ActiveClass<TSelf> value)
        => Volatile.Write(ref _current, value);
}