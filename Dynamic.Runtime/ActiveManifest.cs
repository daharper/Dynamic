using System;
using System.Collections;
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

public sealed record Link(string Title, string Url);

public sealed record Association(ActiveManifest Manifest, Relationship Relationship);

public abstract class ActiveManifest : DynamicObject
{
    private string _title = "";

    protected ActiveManifest(string name = "") => Name = name;

    public string Name { get; set; }

    public string Title
    {
        get => string.IsNullOrWhiteSpace(_title) ? Name : _title;
        set => _title = value;
    }

    public string Description { get; set; } = "";

    public List<string> Aliases { get; } = [];

    public List<Association> Associations { get; } = [];

    public List<Link> Links { get; } = [];

    public Dictionary<string, dynamic> Props { get; } = [];

    public List<string> Notes { get; } = [];

    public List<string> Tags { get; } = [];

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

    public bool HasAlias => Aliases.Count > 0;

    public bool HasProp => Props.Count > 0;

    public bool HasNote => Notes.Count > 0;

    public bool HasTag => Tags.Count > 0;

    public bool HasLink => Links.Count > 0;

    public dynamic Let(string name, dynamic value)
    {
        Props[name] = value;
        return this;
    }

    public string AsStr(string name)
    {
        if (!Props.TryGetValue(name, out var value)) return "";

        return value switch
        {
            null => "null",
            string s => s,
            IEnumerable<int> values => $"[{string.Join(", ", values)}]",
            IEnumerable values => $"[{string.Join(", ", values.Cast<object>())}]",
            _ => value.ToString()
        };
    }

    public dynamic Alias(params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (!Aliases.Contains(alias))
            {
                Aliases.Add(alias);
            }
        }

        return this;
    }


    public bool IsKnownAs(string name) 
        => StringComparer.OrdinalIgnoreCase.Equals(Name, name) 
           || Aliases.Any(alias => StringComparer.OrdinalIgnoreCase.Equals(alias, name));

    public dynamic Note(params string[] notes)
    {
        Notes.AddRange(notes);
        return this;
    }

    public bool IsTag(string tag)
    {
        tag = tag.Trim().ToLowerInvariant();
        return Tags.Contains(tag);
    }

    public dynamic Link(string title, string url)
    {
        Links.Add(new Link(title, url));
        return this;
    }

    public dynamic Tag(params string[] tags)
    {
        Tags.AddRange(tags.Select(t => t.Trim().ToLowerInvariant()));
        return this;
    }

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