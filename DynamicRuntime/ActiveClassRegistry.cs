namespace Dynamic.Runtime;

public static class ActiveClassRegistry<TSelf>
{
    private static ActiveClass<TSelf> _current
        = ActiveClass<TSelf>.Empty;

    public static ActiveClass<TSelf> Current
        => Volatile.Read(ref _current);

    internal static void Replace(ActiveClass<TSelf> value)
        => Volatile.Write(ref _current, value);
}