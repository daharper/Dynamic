using System.Dynamic;

namespace Dynamic.Runtime;

public enum Relationship
{
    Genus,
    Species,

    Lists,
    ListedIn,

    Uses,
    UsedBy
}

public sealed record Association(ActiveNode Node, Relationship Relationship);

public abstract class ActiveNode : DynamicObject
{
    /// <summary>
    /// Gets the collection of associations. 
    /// </summary>
    public List<Association> Associations { get; } = [];

    /// <summary>
    /// Associates the specified objects as genera of this object.
    /// </summary>
    public dynamic Generalizes(params ActiveNode[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.Genus, Relationship.Species);
        }

        return this;
    }

    /// <summary>
    /// Associates the specified objects as species of this object.
    /// </summary>
    public dynamic Specializes(params ActiveNode[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.Species, Relationship.Genus);
        }

        return this;
    }

    /// <summary>
    /// Associates the specified objects as listed members of this object.
    /// </summary>
    public dynamic Lists(params ActiveNode[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.Lists, Relationship.ListedIn);
        }

        return this;
    }

    /// <summary>
    /// Associates this object as a listed member of the specified objects.
    /// </summary>
    public dynamic ListedIn(params ActiveNode[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.ListedIn, Relationship.Lists);
        }

        return this;
    }

    /// <summary>
    /// Associates the specified objects as being used by this object.
    /// </summary>
    public dynamic Uses(params ActiveNode[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.Uses, Relationship.UsedBy);
        }

        return this;
    }

    /// <summary>
    /// Associates this object as being used by the specified objects.
    /// </summary>
    public dynamic UsedBy(params ActiveNode[] manifests)
    {
        foreach (var manifest in manifests)
        {
            Associate(manifest, Relationship.UsedBy, Relationship.Uses);
        }

        return this;
    }
    
    /// <summary>
    /// Gets all genera of this object.
    /// </summary>
    public IEnumerable<dynamic> AllGenera => GetRelated(Relationship.Genus);

    /// <summary>
    /// Gets all species of this object.
    /// </summary>
    public IEnumerable<dynamic> AllSpecies => GetRelated(Relationship.Species);

    /// <summary>
    /// Gets all members of this object.
    /// </summary>
    public IEnumerable<dynamic> AllListed => GetRelated(Relationship.Lists);

    /// <summary>
    /// Gets all objects of which this object is a member.
    /// </summary>
    public IEnumerable<dynamic> AllLists => GetRelated(Relationship.ListedIn);

    /// <summary>
    /// Gets all objects used by this object.
    /// </summary>
    public IEnumerable<dynamic> AllUses => GetRelated(Relationship.Uses);

    /// <summary>
    /// Gets all objects that use this object.
    /// </summary>
    public IEnumerable<dynamic> AllUsages => GetRelated(Relationship.UsedBy);

    /// <summary>
    /// Gets all objects related to this object by the specified relationship.
    /// </summary>
    public IEnumerable<dynamic> GetRelated(Relationship relationship)
        => Associations
            .Where(a => a.Relationship == relationship)
            .Select(a => a.Node);

    /// <summary>
    /// Gets all objects of the specified type related to this object by the specified relationship.
    /// </summary>
    public IEnumerable<T> GetRelated<T>(Relationship relationship)
        where T : ActiveNode
        => GetRelated(relationship).OfType<T>();
    
    /// <summary>
    /// Determines if this object has the specified relationship.
    /// </summary>
    public bool Any(Relationship relationship)
        => Associations.Any(a => a.Relationship == relationship);

    /// <summary>
    /// Indicates whether this object is a genus of any other object.
    /// </summary>
    public bool IsGenus => Any(Relationship.Species);

    /// <summary>
    /// Indicates whether this object is a species of any other object.
    /// </summary>
    public bool IsSpecies => Any(Relationship.Genus);

    /// <summary>
    /// Indicates whether this object is a group with any members.
    /// </summary>
    public bool IsList => Any(Relationship.Lists);

    /// <summary>
    /// Indicates whether this object is a member of any other object.
    /// </summary>
    public bool IsListed => Any(Relationship.ListedIn);

    /// <summary>
    /// Indicates whether this object uses any other object.
    /// </summary>
    public bool IsUser => Any(Relationship.Uses);

    /// <summary>
    /// Indicates whether this object is used by any other object.
    /// </summary>
    public bool IsUsed => Any(Relationship.UsedBy);
    
    /// <summary>
    /// Creates a one-way association from this node to <paramref name="node"/>.
    /// </summary>
    protected void Associate(ActiveNode node, Relationship relationship)
    {
        if (!Associations.Any(a => ReferenceEquals(a.Node, node) && a.Relationship == relationship))
        {
            Associations.Add(new Association(node, relationship));
        }
    }

    /// <summary>
    /// Creates a bidirectional association:
    /// this → node using <paramref name="relationship"/>,
    /// node → this using <paramref name="inverse"/>.
    /// </summary>
    protected void Associate(ActiveNode node, Relationship relationship, Relationship inverse)
    {
        Associate(node, relationship);
        node.Associate(this, inverse);
    }

    /// <summary>
    /// Removes the one-way association from this node to
    /// <paramref name="node"/> having <paramref name="relationship"/>.
    /// </summary>
    protected void Disassociate(ActiveNode node, Relationship relationship) 
        => Associations.RemoveAll(a => ReferenceEquals(a.Node, node) && a.Relationship == relationship);
    
}