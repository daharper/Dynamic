using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Dynamic.Runtime;

/// <summary>
/// A dynamically composable list that combines ordinary <see cref="IList{T}"/>
/// behavior with the runtime capabilities of <see cref="ActiveObject{TSelf}"/>.
/// </summary>
/// <remarks>
/// <para>
/// Dynamic member access can materialize values from member names and arguments.
/// Values produced through dynamic get, invoke, and set operations are added
/// idempotently according to the configured <see cref="IEqualityComparer{T}"/>.
/// If an equivalent value already exists, the existing value is preserved.
/// </para>
/// <para>
/// A factory may be supplied to construct values from dynamic member names.
/// Otherwise, strings use the member name directly, primitive values use CLR
/// conversion, and other types may be constructed using their available
/// constructors.
/// </para>
/// <para>
/// Equality-sensitive list operations use the configured comparer. The collection
/// otherwise behaves as an ordinary mutable <see cref="IList{T}"/> and does not
/// provide synchronization for concurrent access.
/// </para>
/// </remarks>
public class ActiveList<T> : ActiveObject<ActiveList<T>>, IList<T>
{
    private readonly List<T> _items = [];

    private readonly Func<string, T>? _factory;

    private readonly IEqualityComparer<T> _comparer;

    public ActiveList(Func<string, T>? factory = null, IEqualityComparer<T>? comparer = null)
    {
        _factory = factory;
        _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public T this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public void Add(T item) => _items.Add(item);

    public void Clear() => _items.Clear();

    public void CopyTo(T[] array, int arrayIndex) =>
        _items.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() =>
        _items.GetEnumerator();

    public bool Contains(T item) =>
        IndexOf(item) >= 0;

    public int IndexOf(T item)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            if (_comparer.Equals(_items[i], item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, T item) =>
        _items.Insert(index, item);

    public bool Remove(T item)
    {
        var index = IndexOf(item);

        if (index < 0)
        {
            return false;
        }

        _items.RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index) =>
        _items.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        T item;

        if (typeof(T) == typeof(string))
        {
            item = (T)(object)binder.Name;
        }
        else if (typeof(T).IsPrimitive)
        {
            try
            {
                item = (T)Convert.ChangeType(
                    binder.Name,
                    typeof(T));
            }
            catch
            {
                return base.TryGetMember(binder, out result);
            }
        }
        else if (_factory is not null)
        {
            item = _factory(binder.Name);
        }
        else
        {
            try
            {
                item = (T)Activator.CreateInstance(typeof(T), [binder.Name])!;
            }
            catch (MissingMethodException)
            {
                return base.TryGetMember(binder, out result);
            }
        }

        foreach (var existing in _items)
        {
            if (_comparer.Equals(existing, item))
            {
                result = existing;
                return true;
            }
        }

        _items.Add(item);
        result = item;
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (value is not T item)
        {
            return base.TrySetMember(binder, value);
        }

        if (!Contains(item))
        {
            _items.Add(item);
        }

        return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        args ??= [];

        T item;

        if (args.Length == 0 && typeof(T) == typeof(string))
        {
            item = (T)(object)binder.Name;
        }
        else if (args.Length == 0 && typeof(T).IsPrimitive)
        {
            try
            {
                item = (T)Convert.ChangeType(binder.Name, typeof(T));
            }
            catch
            {
                return base.TryInvokeMember(binder, args, out result);
            }
        }
        else if (args.Length == 0 && _factory is not null)
        {
            item = _factory(binder.Name);
        }
        else
        {
            try
            {
                item = (T)Activator.CreateInstance(typeof(T), [binder.Name, .. args])!;
            }
            catch (MissingMethodException)
            {
                return base.TryInvokeMember(binder, args, out result);
            }
        }

        foreach (var existing in _items)
        {
            if (_comparer.Equals(existing, item))
            {
                result = existing;
                return true;
            }
        }

        _items.Add(item);
        result = item;
        return true;
    }
}
