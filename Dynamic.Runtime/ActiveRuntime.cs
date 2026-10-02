using System.Reflection;

namespace Dynamic.Runtime;

public static class ActiveRuntime
{
    private static readonly HashSet<string> RegisteredNamespaces = [];
    private static readonly HashSet<Assembly> RegisteredAssemblies = [];

    public static IEnumerable<string> Namespaces => RegisteredNamespaces;
    public static IEnumerable<Assembly> Assemblies => RegisteredAssemblies;

    public static bool AutoProperties { get; set; } = true;

    public static void Register(params Type[] types)
    {
        foreach (var type in types)
        {
            RegisteredAssemblies.Add(type.Assembly);

            if (type.Namespace is not null)
            {
                RegisteredNamespaces.Add(type.Namespace);
            }
        }
    }
}