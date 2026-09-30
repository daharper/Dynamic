using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Xml.Linq;

namespace Dynamic.Runtime;

public enum Relationship
{
    Child,
    Parent,

    Member,
    MemberOf,

    Uses,
    UsedBy
}

public sealed record Association<TSelf>(TSelf Data, Relationship Relationship);

public abstract class ActiveData<TSelf> : DynamicObject where TSelf : ActiveData<TSelf>
{
    protected ActiveData(string name, TSelf? parent = null)
    {
        Name = name;
        Parent = parent;
    }

    public string Name { get; set; }

    public TSelf? Parent { get; private set; }

    public List<Association<TSelf>> Associations { get; } = [];

    public IEnumerable<TSelf> Children => Related(Relationship.Child);

    public IEnumerable<TSelf> Members => Related(Relationship.Member);

    public IEnumerable<TSelf> MemberOf => Related(Relationship.MemberOf);

    public IEnumerable<TSelf> UsedBy => Related(Relationship.UsedBy);

    public bool HasParent => Parent is not null;

    public bool Has(Relationship relationship)
        => Associations.Any(a => a.Relationship == relationship);

    public IEnumerable<TSelf> Related(Relationship relationship)
        => Associations.Where(a => a.Relationship == relationship).Select(a => a.Data);

    public TSelf Owns(params TSelf[] data)
    {
        foreach (var item in data)
        {
            // Ownership implies membership.
            With(item);

            // Retract only the structural relationship
            // contradicted by the new ownership.
            if (item.Parent is not null && item.Parent != this)
            {
                item.Parent.Disassociate(item, Relationship.Child);
            }

            item.Parent = (TSelf)this;

            Associate(item, Relationship.Child);
        }

        return (TSelf)this;
    }

    public TSelf With(params TSelf[] data)
    {
        foreach (var item in data)
        {
            Associate(item, Relationship.Member, Relationship.MemberOf);
        }

        return (TSelf)this;
    }

    public TSelf Uses(params TSelf[] data)
    {
        foreach (var item in data)
        {
            Associate(item, Relationship.Uses, Relationship.UsedBy);
        }

        return (TSelf)this;
    }

    protected void Associate(TSelf data, Relationship relationship)
    {
        if (!HasAssociation(data, relationship))
            Associations.Add(new(data, relationship));
    }

    protected void Associate(TSelf data, Relationship relationship, Relationship inverse)
    {
        Associate(data, relationship);
        data.Associate((TSelf)this, inverse);
    }

    protected void Disassociate(TSelf data, Relationship relationship)
        => Associations.RemoveAll(a => ReferenceEquals(a.Data, data) && a.Relationship == relationship);
    

    private bool HasAssociation(TSelf data, Relationship relationship) 
        => Associations.Any(a => ReferenceEquals(a.Data, data) && a.Relationship == relationship);
}
