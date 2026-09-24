using System.Dynamic;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Dynamic.Runtime;

/// <summary>
/// Provides the base implementation for objects whose members can be
/// resolved and invoked dynamically at runtime.
/// </summary>
/// <typeparam name="TSelf"> The concrete active object type.</typeparam>
public abstract class ActiveObject<TSelf> : DynamicObject where TSelf : ActiveObject<TSelf>
{
    private ActiveObjectClass<TSelf> _objectClass = ActiveObjectClass<TSelf>.Empty;

    private static readonly MethodInfo[] ClrMethods = typeof(TSelf)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(static method => !method.IsSpecialName)
            .ToArray();

    private static readonly string[] ClrMethodNames =
        typeof(TSelf)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(static method => !method.IsSpecialName)
            .Select(static method => method.Name)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

    private static readonly string[] ClrPropertyNames =
        typeof(TSelf)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(static property => property.Name)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

    private readonly Lock _slotGate = new();

    private readonly Dictionary<string, object?> _slots = new(StringComparer.Ordinal);

    protected TSelf Self => (TSelf)this;

    protected ActiveClass<TSelf> Class => ActiveClassRegistry<TSelf>.Current;

    public FreezeMode Freeze { get; set; } = FreezeMode.None;

    public void ClassEval(Action<ActiveClassBuilder<TSelf>> eval)
    {
        var current = ActiveClassRegistry<TSelf>.Current;
        var builder = current.ToBuilder();

        eval(builder);

        ActiveClassRegistry<TSelf>.Replace(builder.Build());
    }

    public void ClassEval(string name, DynamicMethod<TSelf> method)
    {
        var current = ActiveClassRegistry<TSelf>.Current;
        var builder = current.ToBuilder();

        builder.Method(name, method);

        ActiveClassRegistry<TSelf>.Replace(builder.Build());
    }

    public void ClassEval(string source)
    {
        var current = ActiveClassRegistry<TSelf>.Current;
        var next = RuntimeCompiler.CompileClass(current, source);

        ActiveClassRegistry<TSelf>.Replace(next);
    }

    public void Eval(Action<ActiveObjectClassBuilder<TSelf>> eval)
    {
        EnsureObjectClassMutable();

        var current = Volatile.Read(ref _objectClass);
        var builder = current.ToBuilder();

        eval(builder);

        Volatile.Write(ref _objectClass, builder.Build());
    }

    public void Eval(string source)
    {
        EnsureObjectClassMutable();

        var current = Volatile.Read(ref _objectClass);
        var next = RuntimeCompiler.CompileInstance(Self, current, source);

        Volatile.Write(ref _objectClass, next);
    }

    internal object? GetRuntimeSlot(string name)
    {
        lock (_slotGate)
        {
            return _slots.GetValueOrDefault(name);
        }
    }

    internal void SetRuntimeSlot(string name, object? value)
    {
        lock (_slotGate)
        {
            _slots[name] = value;
        }
    }

    internal bool TryDispatchMessage(RuntimeMessage message, out object? result)
    {
        return TryInvokeRuntimeMember(message, out result) || 
               TryInvokeClrMember(message, out result);
    }

    private bool TryInvokeClrMember(RuntimeMessage message, out object? result)
    {
        if (!RuntimeMethodBinder.TryBind(ClrMethods, message, out var binding))
        {
            result = null;
            return false;
        }

        try
        {
            result = binding.Method.Invoke(Self, binding.Arguments);
            return true;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw; // satisfy control-flow analysis
        }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        var message = new RuntimeMessage(binder.Name, args ?? []);
        return TryInvokeRuntimeMember(message, out result);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var objectClass = Volatile.Read(ref _objectClass);

        if (objectClass.TryGetProperty(binder.Name, out var objectProperty))
        {
            if (objectProperty.Getter is null)
            {
                result = null;
                return false;
            }

            result = objectProperty.Getter(Self);
            return true;
        }

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        if (activeClass.TryGetProperty(binder.Name, out var classProperty))
        {
            if (classProperty.Getter is null)
            {
                result = null;
                return false;
            }

            result = classProperty.Getter(Self);

            return true;
        }

        if (Freeze is FreezeMode.Partial or FreezeMode.Fully)
        {
            result = null;
            return false;
        }

        var property = AddObjectProperty(binder.Name);

        result = property.Getter!(Self);

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        var objectClass = Volatile.Read(ref _objectClass);

        if (objectClass.TryGetProperty(binder.Name, out var objectProperty))
        {
            if (Freeze == FreezeMode.Fully) return false;
            if (objectProperty.Setter is null) return false;
            
            objectProperty.Setter(Self, value);
            return true;
        }

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        if (activeClass.TryGetProperty(binder.Name, out var classProperty))
        {
            if (Freeze == FreezeMode.Fully) return false;
            if (classProperty.Setter is null) return false;

            classProperty.Setter(Self, value);
            return true;
        }

        if (Freeze is FreezeMode.Partial or FreezeMode.Fully) return false;

        var property = AddObjectProperty(binder.Name);

        property.Setter!(Self, value);

        return true;
    }

    public IReadOnlyCollection<string> ObjectMethods()
    {
        var objectClass = Volatile.Read(ref _objectClass);

        return objectClass.MethodNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<string> ClassMethods()
    {
        var activeClass = ActiveClassRegistry<TSelf>.Current;

        return activeClass.MethodNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<string> Methods()
    {
        var objectClass = Volatile.Read(ref _objectClass);

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        return objectClass.MethodNames
            .Concat(activeClass.MethodNames)
            .Concat(ClrMethodNames)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<string> ObjectProperties()
    {
        var objectClass = Volatile.Read(ref _objectClass);

        return objectClass.PropertyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<string> ClassProperties()
    {
        var activeClass = ActiveClassRegistry<TSelf>.Current;

        return activeClass.PropertyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<string> Properties()
    {
        var objectClass = Volatile.Read(ref _objectClass);

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        return objectClass.PropertyNames
            .Concat(activeClass.PropertyNames)
            .Concat(ClrPropertyNames)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
    }

    internal bool TryInvokeRuntimeMember(RuntimeMessage message, out object? result)
    {
        var objectClass = Volatile.Read(ref _objectClass);

        if (objectClass.TryGetMethod(message.Name, out var objectMethod))
        {
            result = objectMethod(Self, message.Arguments);

            return true;
        }

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        if (activeClass.TryGetMethod(message.Name, out var classMethod))
        {
            result = classMethod(Self, message.Arguments);

            return true;
        }

        result = null;
        return false;
    }

    public object? Send(string name, params object?[]? args)
    {
        var message = new RuntimeMessage(name, args ?? [null]);

        if (TryDispatchMessage(message, out var result))
        {
            return result;
        }

        return MethodMissing(message);
    }

    public T Send<T>(string name, params object?[]? args)
    {
        return (T)Send(name, args)!;
    }

    public object? Send(string message)
    {
        var runtimeMessage = RuntimeMessageParser.Parse(message);

        if (TryDispatchMessage(runtimeMessage, out var result))
        {
            return result;
        }

        return MethodMissing(runtimeMessage);
    }

    protected virtual object? MethodMissing(RuntimeMessage message)
    {
        throw new MissingMethodException(
            $"Method '{message.Name}' was not found on " +
            $"'{typeof(TSelf).Name}'.");
    }

    public bool HasMethod(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var objectClass = Volatile.Read(ref _objectClass);

        if (objectClass.MethodNames.Any(n => string.Equals(n, name, StringComparison.Ordinal)))
        {
            return true;
        }

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        if (activeClass.MethodNames.Any(n => string.Equals(n, name, StringComparison.Ordinal)))
        {
            return true;
        }

        return ClrMethodNames.Contains(name, StringComparer.Ordinal);
    }

    public bool HasProperty(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var objectClass = Volatile.Read(ref _objectClass);

        if (objectClass.PropertyNames.Any(propertyName => string.Equals(propertyName, name, StringComparison.Ordinal)))
        {
            return true;
        }

        var activeClass = ActiveClassRegistry<TSelf>.Current;

        if (activeClass.PropertyNames.Any(propertyName => string.Equals(propertyName, name, StringComparison.Ordinal)))
        {
            return true;
        }

        return ClrPropertyNames.Contains(name, StringComparer.Ordinal);
    }

    private DynamicProperty<TSelf> AddObjectProperty(string name)
    {
        var property = new DynamicProperty<TSelf>
            {
                Getter = self => self.GetRuntimeSlot(name),
                Setter = (self, value) => self.SetRuntimeSlot(name, value)
            };

        var current = Volatile.Read(ref _objectClass);
        var builder = current.ToBuilder();

        builder.Property(name, property);

        Volatile.Write(ref _objectClass, builder.Build());

        return property;
    }

    private void EnsureObjectClassMutable()
    {
        if (Freeze == FreezeMode.Fully)
        {
            throw new InvalidOperationException("This object's runtime structure is fully frozen.");
        }
    }
}