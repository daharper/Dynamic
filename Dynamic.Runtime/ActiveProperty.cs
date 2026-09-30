using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamic.Runtime;

public class ActiveProperty<T> : ActiveData<ActiveProperty<T>>
{
    public ActiveProperty(string name = "", T value = default(T), T? parent = default) : base(name, parent)
    {
        Value = value;
    }

    public T Value { get; set; }

    public static implicit operator T(ActiveProperty<T> property) => property.Value;

    public static implicit operator ActiveProperty<T>(T value) => new("", value);
}