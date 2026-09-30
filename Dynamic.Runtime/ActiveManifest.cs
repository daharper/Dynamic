using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

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

public sealed record Association(ActiveManifest Manifest, Relationship Relationship);

public abstract class ActiveManifest : DynamicObject
{
    protected ActiveManifest(string name = "") => Name = name;

    public string Name { get; set; }

    public List<Association> Associations { get; } = [];

    public IEnumerable<dynamic> AllMembers => Related(Relationship.Member);

    public IEnumerable<dynamic> MemberOf => Related(Relationship.MemberOf);

    public IEnumerable<dynamic> AllUses => Related(Relationship.Uses);

    public IEnumerable<dynamic> UsedBy => Related(Relationship.UsedBy);

    public IEnumerable<dynamic> Related(Relationship relationship) 
        => Associations.Where(a => a.Relationship == relationship).Select(a => a.Manifest);

    public IEnumerable<T> Related<T>(Relationship relationship) where T : ActiveManifest 
        => Related(relationship).OfType<T>();

    public bool Has(Relationship relationship)
        => Associations.Any(a => a.Relationship == relationship);

    public bool HasMember => Has(Relationship.Member);

    public bool IsMember => Has(Relationship.MemberOf);

    public bool HasUses => Has(Relationship.Uses);

    public bool IsUsed => Has(Relationship.UsedBy);

    public dynamic Uses(params ActiveManifest[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.Uses, Relationship.UsedBy);
        }

        return this;
    }

    public dynamic Includes(params ActiveManifest[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.Member, Relationship.MemberOf);
        }

        return this;
    }

    protected void Associate(ActiveManifest manifest, Relationship relationship)
    {
        if (!Associations.Any(a => ReferenceEquals(a.Manifest, manifest) && a.Relationship == relationship))
        {
            Associations.Add(new Association(manifest, relationship));
        }
    }

    protected void Associate(ActiveManifest manifest, Relationship relationship, Relationship inverse)
    {
        Associate(manifest, relationship);
        manifest.Associate(this, inverse);
    }

    protected void Disassociate(ActiveManifest manifest, Relationship relationship) 
        => Associations.RemoveAll(a => ReferenceEquals(a.Manifest, manifest) && a.Relationship == relationship);
    
}