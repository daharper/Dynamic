using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Xml.Linq;

namespace Dynamic.Runtime;

public abstract class ActiveData<T> : ActiveManifest where T : ActiveData<T>
{
    protected ActiveData(string name = "", T? parent = null) : base(name)
    {
        Parent = parent;
    }

    public T? Parent { get; private set; }

    public bool HasParent => Parent is not null;

    public bool HasChild => Has(Relationship.Child);

    public IEnumerable<T> Children => Related<T>(Relationship.Child);

    public T Generalizes(params T[] data)
    {
        foreach (var item in data)
        {
            if (item.Parent is not null && item.Parent != this)
            {
                item.Parent.Disassociate(item, Relationship.Child);
            }

            item.Parent = (T)this;

            Associate(item, Relationship.Child);
        }

        return (T)this;
    }
}