using System.Collections;

namespace Dynamic.Runtime;

/// <summary>
/// Represents a link with a title and URL.
/// </summary>
public sealed record ActiveLink(string Title, string Url);

/// <summary>
/// Represents metadata that contains an identity and various associated properties,
/// aliases, notes, tags, and links. Provides functionality to manage and query these
/// metadata components dynamically.
/// </summary>
public class ActiveMetadata : ActiveNode
{
    /// <summary>
    /// Gets or sets the intent.
    /// </summary>
    public string Intent { get; set; } = "";
    
    /// <summary>
    /// Gets the collection of aliases.
    /// </summary>
    public List<string> MetaAliases { get; } = [];

    /// <summary>
    /// Gets the collection of links.
    /// </summary>
    public List<ActiveLink> MetaLinks { get; } = [];

    /// <summary>
    /// Gets the dictionary of properties.
    /// </summary>
    public Dictionary<string, dynamic> MetaProperties { get; } = [];

    /// <summary>
    /// Gets the collection of notes.
    /// </summary>
    public List<string> MetaNotes { get; } = [];

    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    public List<string> MetaTags { get; } = [];

    /// <summary>
    /// Indicates whether this object has any aliases.
    /// </summary>
    public bool AnyMetaAlias => MetaAliases.Count > 0;

    /// <summary>
    /// Indicates whether this object has any properties.
    /// </summary>
    public bool AnyMetaProperty => MetaProperties.Count > 0;

    /// <summary>
    /// Indicates whether this object has any notes.
    /// </summary>
    public bool AnyMetaNote => MetaNotes.Count > 0;

    /// <summary>
    /// Indicates whether this object has any tags.
    /// </summary>
    public bool AnyMetaTag => MetaTags.Count > 0;

    /// <summary>
    /// Indicates whether this object has any links.
    /// </summary>
    public bool AnyMetaLink => MetaLinks.Count > 0;
    
    /// <summary>
    /// Determines if the given name corresponds to the identity or any of the meta aliases.
    /// </summary>
    public bool HasMetaAlias(string name) 
        => MetaAliases.Any(alias => StringComparer.OrdinalIgnoreCase.Equals(alias, name));
    
    /// <summary>
    /// Indicates whether the specified meta property exists.
    /// </summary>
    public bool HasMetaProperty(string name)
        => MetaProperties.ContainsKey(name);
    
    public dynamic Aka(params string[] aliases)
        => MetaAlias(aliases);
    
    public dynamic MetaAlias(params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (!MetaAliases.Contains(alias, StringComparer.OrdinalIgnoreCase))
            {
                MetaAliases.Add(alias);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds one or more meta-notes to the current metadata object.
    /// </summary>
    public dynamic MetaNote(params string[] notes)
    {
        MetaNotes.AddRange(notes);
        return this;
    }

    /// <summary>
    /// Adds a new metadata link with the specified title and URL to the collection of meta links.
    /// </summary>
    public dynamic MetaLink(string title, string url)
    {
        MetaLinks.Add(new ActiveLink(title, url));
        return this;
    }

    /// <summary>
    /// Adds one or more meta tags to the current metadata object.
    /// </summary>
    public dynamic MetaTag(params string[] tags)
    {
        MetaTags.AddRange(tags.Select(t => t.Trim().ToLowerInvariant()));
        return this;
    }

    /// <summary>
    /// Checks if the given tag exists in the list of metadata tags.
    /// </summary>
    public bool IsMetaTag(string tag)
    {
        tag = tag.Trim().ToLowerInvariant();
        return MetaTags.Contains(tag);
    }
    
    /// <summary>
    /// Adds or updates a metadata property with the specified name and value.
    /// </summary>
    public dynamic Let(string name, dynamic value)
    {
        MetaProperties[name] = value;
        return this;
    }

    /// <summary>
    /// Tries to retrieve a property value as a string.
    /// </summary>
    public string AsStr(string name)
    {
        if (!MetaProperties.TryGetValue(name, out var value)) return "";

        return value switch
        {
            null => "null",
            string s => s,
            IEnumerable<int> values => $"[{string.Join(", ", values)}]",
            IEnumerable values => $"[{string.Join(", ", values.Cast<object>())}]",
            _ => value.ToString()
        };
    }
}