using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dynamic.Runtime;

/// <summary>
/// Provides runtime compilation of dynamically defined C# code.
/// </summary>
public static class RuntimeCompiler
{
    private static int _assemblyId;

    public static ActiveClass<TSelf> CompileClass<TSelf>(ActiveClass<TSelf> current, string source)
        where TSelf : ActiveObject<TSelf>
    {
        var members = ParseMembers(source);

        if (members.Methods.Count == 0 && members.Properties.Count == 0)
        {
            return current;
        }

        var runtimePropertyNames = current.PropertyNames
            .Concat(members.Properties.Select(static property => property.Identifier.ValueText))
            .ToHashSet(StringComparer.Ordinal);

        var runtimeMethodNames = current.MethodNames
            .Concat(members.Methods.Select(static method => method.Identifier.ValueText))
            .ToHashSet(StringComparer.Ordinal);

        var assemblyId = Interlocked.Increment(ref _assemblyId);

        var assemblyName = $"DynamicRuntime_Class_{typeof(TSelf).Name}_{assemblyId}";

        var generatedTypeName = $"__ClassPatch_{assemblyId}";

        var generatedSource = GenerateSource<TSelf>(
            generatedTypeName,
            members.Methods,
            members.Properties,
            runtimePropertyNames,
            runtimeMethodNames);


        var syntaxTree = CSharpSyntaxTree.ParseText(generatedSource, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            GetMetadataReferences<TSelf>(),
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel:
                OptimizationLevel.Release,
                nullableContextOptions:
                NullableContextOptions.Enable,
                allowUnsafe: true));

        using var pe = new MemoryStream();

        var emit = compilation.Emit(pe);

        if (!emit.Success)
        {
            throw CreateCompilationException(source, generatedSource, emit.Diagnostics);
        }

        pe.Position = 0;

        var assembly = AssemblyLoadContext.Default.LoadFromStream(pe);

        var generatedType = assembly.GetType($"DynamicRuntime.Generated.{generatedTypeName}", throwOnError: true)!;

        var builder = current.ToBuilder();

        InstallMethods(generatedType, members.Methods, builder);

        InstallProperties(generatedType, members.Properties, builder);

        return builder.Build();
    }

    public static ActiveObjectClass<TSelf> CompileInstance<TSelf>(TSelf self, ActiveObjectClass<TSelf> current,
        string source)
        where TSelf : ActiveObject<TSelf>
    {
        var members = ParseMembers(source);

        if (members.Methods.Count == 0 && members.Properties.Count == 0)
        {
            return current;
        }

        var runtimePropertyNames = ActiveClassRegistry<TSelf>.Current
            .PropertyNames
            .Concat(current.PropertyNames)
            .Concat(members.Properties.Select(static property => property.Identifier.ValueText))
            .ToHashSet(StringComparer.Ordinal);

        var runtimeMethodNames = ActiveClassRegistry<TSelf>.Current
            .MethodNames
            .Concat(current.MethodNames)
            .Concat(members.Methods.Select(static method => method.Identifier.ValueText))
            .ToHashSet(StringComparer.Ordinal);

        var assemblyId = Interlocked.Increment(ref _assemblyId);

        var assemblyName = $"DynamicRuntime_Instance_{typeof(TSelf).Name}_{assemblyId}";

        var generatedTypeName = $"__InstancePatch_{assemblyId}";

        var generatedSource = GenerateSource<TSelf>(
            generatedTypeName,
            members.Methods,
            members.Properties,
            runtimePropertyNames,
            runtimeMethodNames);

        var syntaxTree = CSharpSyntaxTree.ParseText(generatedSource, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            GetMetadataReferences<TSelf>(),
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel:
                OptimizationLevel.Release,
                nullableContextOptions:
                NullableContextOptions.Enable,
                allowUnsafe: true));

        using var pe = new MemoryStream();

        var emit = compilation.Emit(pe);

        if (!emit.Success)
        {
            throw CreateCompilationException(source, generatedSource, emit.Diagnostics);
        }

        pe.Position = 0;

        var assembly = AssemblyLoadContext.Default.LoadFromStream(pe);

        var generatedType = assembly.GetType($"DynamicRuntime.Generated.{generatedTypeName}", throwOnError: true)!;

        var builder = current.ToBuilder();

        InstallMethods(generatedType, members.Methods, builder);

        InstallProperties(generatedType, members.Properties, builder);

        return builder.Build();
    }

    private static void InstallMethods<TSelf>(
        Type generatedType,
        IReadOnlyList<MethodDeclarationSyntax> methods,
        ActiveClassBuilder<TSelf> builder)
    {
        foreach (var method in methods)
        {
            var invokeName = $"__Invoke_{method.Identifier.ValueText}";

            var invoker = generatedType.GetMethod(
                              invokeName,
                              BindingFlags.Public |
                              BindingFlags.Static)
                          ?? throw new InvalidOperationException(
                              $"Generated method '{invokeName}' was not found.");

            var implementation = (DynamicMethod<TSelf>)
                invoker.CreateDelegate(typeof(DynamicMethod<TSelf>));

            builder.Method(method.Identifier.ValueText, implementation);
        }
    }

    private static void InstallMethods<TSelf>(
        Type generatedType,
        IReadOnlyList<MethodDeclarationSyntax> methods,
        ActiveObjectClassBuilder<TSelf> builder)
    {
        foreach (var method in methods)
        {
            var invokeName = $"__Invoke_{method.Identifier.ValueText}";

            var invoker = generatedType.GetMethod(
                              invokeName,
                              BindingFlags.Public |
                              BindingFlags.Static)
                          ?? throw new InvalidOperationException($"Generated method '{invokeName}' was not found.");

            var implementation = (DynamicMethod<TSelf>)invoker.CreateDelegate(typeof(DynamicMethod<TSelf>));

            builder.Method(method.Identifier.ValueText, implementation);
        }
    }

    private static void InstallProperties<TSelf>(
        Type generatedType,
        IReadOnlyList<PropertyDeclarationSyntax> properties,
        ActiveClassBuilder<TSelf> builder)
        where TSelf : ActiveObject<TSelf>
    {
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];

            var name = property.Identifier.ValueText;

            if (IsAutoProperty(property))
            {
                Func<TSelf, object?> slotGetter = self => self.GetRuntimeSlot(name);

                Action<TSelf, object?>? slotSetter = HasSetter(property)
                    ? (self, value) => self.SetRuntimeSlot(name, value)
                    : null;

                builder.Property(name, new DynamicProperty<TSelf>
                {
                    Getter = slotGetter,
                    Setter = slotSetter
                });

                continue;
            }

            var getterName = $"__GetProperty_{i}_{name}";

            var getterMethod = generatedType.GetMethod(
                                   getterName,
                                   BindingFlags.Public |
                                   BindingFlags.Static)
                               ?? throw new InvalidOperationException(
                                   $"Generated getter '{getterName}' was not found.");

            var computedGetter = (Func<TSelf, object?>)getterMethod.CreateDelegate(typeof(Func<TSelf, object?>));

            builder.Property(name, new DynamicProperty<TSelf> { Getter = computedGetter });
        }
    }

    private static void InstallProperties<TSelf>(
        Type generatedType,
        IReadOnlyList<PropertyDeclarationSyntax> properties,
        ActiveObjectClassBuilder<TSelf> builder)
        where TSelf : ActiveObject<TSelf>
    {
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];

            var name = property.Identifier.ValueText;

            if (IsAutoProperty(property))
            {
                Func<TSelf, object?> slotGetter = self => self.GetRuntimeSlot(name);

                Action<TSelf, object?>? slotSetter = HasSetter(property)
                    ? (self, value) => self.SetRuntimeSlot(name, value)
                    : null;

                builder.Property(name, new DynamicProperty<TSelf>
                {
                    Getter = slotGetter,
                    Setter = slotSetter
                });

                continue;
            }

            var getterName = $"__GetProperty_{i}_{name}";

            var getterMethod = generatedType.GetMethod(
                                   getterName,
                                   BindingFlags.Public |
                                   BindingFlags.Static)
                               ?? throw new InvalidOperationException(
                                   $"Generated getter '{getterName}' was not found.");

            var computedGetter = (Func<TSelf, object?>)getterMethod.CreateDelegate(typeof(Func<TSelf, object?>));

            builder.Property(name, new DynamicProperty<TSelf> { Getter = computedGetter });
        }
    }

    private static ParsedMembers ParseMembers(string source)
    {
        var wrapped = $$"""
                        class __Input
                        {
                            {{source}}
                        }
                        """;

        var tree = CSharpSyntaxTree.ParseText(wrapped, new CSharpParseOptions(LanguageVersion.Preview));

        var root = tree.GetCompilationUnitRoot();

        var parseErrors = tree.GetDiagnostics()
            .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();

        if (parseErrors.Length > 0)
        {
            throw new RuntimeCompilationException(source, wrapped, parseErrors);
        }

        var type = root.DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(static declaration => declaration.Identifier.ValueText == "__Input");

        var methods = new List<MethodDeclarationSyntax>();

        var properties = new List<PropertyDeclarationSyntax>();

        foreach (var member in type.Members)
        {
            switch (member)
            {
                case MethodDeclarationSyntax method:
                    methods.Add(method);
                    break;

                case PropertyDeclarationSyntax property:
                    properties.Add(property);
                    break;

                default:
                    throw new NotSupportedException(
                        $"RuntimeCompiler does not yet support member type '{member.Kind()}'.");
            }
        }

        return new ParsedMembers(methods, properties);
    }

    private static string GenerateSource<TSelf>(
        string generatedTypeName,
        IReadOnlyList<MethodDeclarationSyntax> methods,
        IReadOnlyList<PropertyDeclarationSyntax> properties,
        IReadOnlySet<string> runtimePropertyNames,
        IReadOnlySet<string> runtimeMethodNames)
        where TSelf : ActiveObject<TSelf>
    {
        var selfType = GetCSharpTypeName(typeof(TSelf));

        var generatedMembers = new StringBuilder();

        for (var i = 0; i < methods.Count; i++)
        {
            generatedMembers.AppendLine(
                GenerateMethod<TSelf>(
                    methods[i],
                    i,
                    selfType,
                    runtimePropertyNames,
                    runtimeMethodNames));
        }

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];

            if (IsAutoProperty(property)) continue;

            generatedMembers.AppendLine(
                GenerateProperty<TSelf>(
                    property,
                    i,
                    selfType,
                    runtimePropertyNames,
                    runtimeMethodNames));
        }

        return $$"""
                 #nullable enable

                 using System;
                 using System.Collections.Generic;
                 using System.IO;
                 using System.Linq;
                 using System.Threading;
                 using System.Threading.Tasks;
                 using System.Reflection;
                 using System.Runtime.Loader;
                 using Microsoft.CodeAnalysis;
                 using Microsoft.CodeAnalysis.CSharp;

                 namespace DynamicRuntime.Generated;

                 public static class {{generatedTypeName}}
                 {
                 {{Indent(generatedMembers.ToString(), 1)}}
                 }
                 """;
    }

    private static string GenerateMethod<TSelf>(
        MethodDeclarationSyntax method,
        int index,
        string selfType,
        IReadOnlySet<string> runtimePropertyNames,
        IReadOnlySet<string> runtimeMethodNames)
    {
        ValidateMethod(method);

        var name = method.Identifier.ValueText;

        var implementationName = $"__Implementation_{index}_{name}";

        var invokeName = $"__Invoke_{name}";

        var returnType = method.ReturnType.ToFullString().Trim();

        var unsafeModifier =
            method.Modifiers.Any(static modifier => modifier.IsKind(SyntaxKind.UnsafeKeyword))
                ? "unsafe "
                : string.Empty;

        var parameters = method.ParameterList.Parameters;

        var implementationParameters =
            new List<string>
            {
                $"{selfType} self"
            };

        implementationParameters.AddRange(parameters.Select(static parameter => parameter.ToFullString().Trim()));

        var invocationArguments = new List<string>();

        for (var i = 0; i < parameters.Count; i++)
        {
            var parameter = parameters[i];

            var parameterType =
                parameter.Type?.ToFullString().Trim()
                ?? throw new NotSupportedException($"Parameter '{parameter.Identifier}' must have an explicit type.");

            invocationArguments.Add($"({parameterType})args[{i}]!");
        }

        var rewriter = new SelfMemberRewriter(typeof(TSelf), runtimePropertyNames, runtimeMethodNames);

        var rewritten = (MethodDeclarationSyntax)rewriter.Visit(method)!;

        var implementationBody = rewritten.Body is not null
            ? rewritten.Body.ToFullString()
            : $"{{ return {rewritten.ExpressionBody!.Expression}; }}";

        var invokeArguments = invocationArguments.Count == 0
            ? "self"
            : $"self, {string.Join(", ", invocationArguments)}";

        var argumentValidation = $$"""
                                           if (args.Length != {{parameters.Count}})
                                           {
                                               throw new global::System.Reflection.TargetParameterCountException(
                                                   "Method '{{name}}' expects {{parameters.Count}} argument(s), but received " + args.Length + ".");
                                           }
                                   """;

        var wrapperBody =
            IsVoid(method)
                ? $$"""
                            {{argumentValidation}}

                            {{implementationName}}({{invokeArguments}});
                            return null;
                    """
                : $$"""
                            {{argumentValidation}}

                            return {{implementationName}}({{invokeArguments}});
                    """;

        return $$"""

                     private static {{unsafeModifier}}{{returnType}} {{implementationName}}(
                         {{string.Join(", ", implementationParameters)}})
                     {{implementationBody}}

                     public static object? {{invokeName}}(
                         {{selfType}} self,
                         object?[] args)
                     {
                 {{Indent(wrapperBody, 2)}}
                     }

                 """;
    }

    private static string GenerateProperty<TSelf>(
        PropertyDeclarationSyntax property,
        int index,
        string selfType,
        IReadOnlySet<string> runtimePropertyNames,
        IReadOnlySet<string> runtimeMethodNames)
    {
        var name =
            property.Identifier.ValueText;

        var getter =
            property.AccessorList?
                .Accessors
                .FirstOrDefault(static accessor =>
                    accessor.IsKind(
                        SyntaxKind.GetAccessorDeclaration));

        if (getter is null)
        {
            throw new NotSupportedException($"Property '{name}' must have a getter.");
        }

        var rewriter = new SelfMemberRewriter(typeof(TSelf), runtimePropertyNames, runtimeMethodNames);

        var rewrittenGetter = (AccessorDeclarationSyntax)rewriter.Visit(getter)!;

        string getterBody;

        if (rewrittenGetter.ExpressionBody is not null)
        {
            getterBody = $"{{ return {rewrittenGetter.ExpressionBody.Expression}; }}";
        }
        else if (rewrittenGetter.Body is not null)
        {
            getterBody = rewrittenGetter.Body.ToFullString();
        }
        else
        {
            throw new NotSupportedException($"Property '{name}' must have a getter body.");
        }

        return $$"""

                     public static object? __GetProperty_{{index}}_{{name}}(
                         {{selfType}} self)
                     {{getterBody}}

                 """;
    }

    private static bool IsAutoProperty(PropertyDeclarationSyntax property)
    {
        if (property.AccessorList is null)
            return false;

        return property.AccessorList
            .Accessors
            .All(static accessor =>
                accessor.Body is null &&
                accessor.ExpressionBody is null);
    }

    private static bool HasSetter(PropertyDeclarationSyntax property)
    {
        return property.AccessorList?
                   .Accessors
                   .Any(static accessor =>
                       accessor.IsKind(
                           SyntaxKind.SetAccessorDeclaration))
               == true;
    }

    private static void ValidateMethod(MethodDeclarationSyntax method)
    {
        if (method.TypeParameterList is not null)
        {
            throw new NotSupportedException(
                $"Generic runtime method '{method.Identifier.ValueText}' is not supported yet.");
        }

        foreach (var parameter
                 in method.ParameterList.Parameters)
        {
            if (parameter.Modifiers.Any(static modifier =>
                    modifier.Kind() is
                        SyntaxKind.RefKeyword or
                        SyntaxKind.OutKeyword or
                        SyntaxKind.InKeyword))
            {
                throw new NotSupportedException(
                    $"ref/out/in parameters are not supported yet: '{parameter.Identifier}'.");
            }
        }

        if (method.Body is null &&
            method.ExpressionBody is null)
        {
            throw new NotSupportedException(
                $"Method '{method.Identifier.ValueText}' must have a body.");
        }
    }

    private static bool IsVoid(MethodDeclarationSyntax method)
    {
        return method.ReturnType
                   is PredefinedTypeSyntax predefined &&
               predefined.Keyword.IsKind(
                   SyntaxKind.VoidKeyword);
    }

    private static IReadOnlyList<MetadataReference> GetMetadataReferences<TSelf>()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic) continue;

            if (string.IsNullOrWhiteSpace(assembly.Location))
            {
                continue;
            }

            paths.Add(assembly.Location);
        }

        AddAssembly(paths, typeof(object).Assembly);

        AddAssembly(paths, typeof(TSelf).Assembly);

        AddAssembly(paths, typeof(ActiveObject<>).Assembly);

        AddAssembly(paths, typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly);

        return paths.Select(static path => MetadataReference.CreateFromFile(path)).ToArray();
    }

    private static void AddAssembly(HashSet<string> paths, Assembly assembly)
    {
        if (assembly.IsDynamic) return;

        if (string.IsNullOrWhiteSpace(assembly.Location))
        {
            return;
        }

        paths.Add(assembly.Location);
    }

    private static string GetCSharpTypeName(Type type)
    {
        if (type.IsGenericType)
        {
            throw new NotSupportedException($"Generic active types are not supported yet: '{type}'.");
        }

        var name = type.FullName ?? throw new InvalidOperationException($"Type '{type}' has no FullName.");

        return "global::" + name.Replace('+', '.');
    }

    private static RuntimeCompilationException
        CreateCompilationException(string source, string generatedSource, IEnumerable<Diagnostic> diagnostics)
    {
        var errors = diagnostics
            .Where(static diagnostic =>
                diagnostic.Severity ==
                DiagnosticSeverity.Error)
            .ToArray();

        return new RuntimeCompilationException(source, generatedSource, errors);
    }

    private static string Indent(string text, int level)
    {
        var indentation = new string(' ', level * 4);

        var lines = text.Replace("\r\n", "\n").Split('\n');

        return string.Join(Environment.NewLine,
            lines.Select(line => line.Length == 0 ? string.Empty : indentation + line));
    }

    public static object? Evaluate(string source)
    {
        var assemblyName = $"DynamicRuntime.Eval.{Guid.NewGuid():N}";

        var expression = SyntaxFactory.ParseExpression(source);

        var isExpression = !expression.ContainsDiagnostics;

        var body = isExpression ? $"return {source};" : source;

        var generatedSource =
            $$"""
              #nullable enable

              using System;
              using System.Collections.Generic;
              using System.IO;
              using System.Linq;

              public static class __DynamicEval
              {
                  public static object? Invoke()
                  {
              {{Indent(body, 2)}}
                  }
              }
              """;

        var syntaxTree = CSharpSyntaxTree.ParseText(generatedSource, new CSharpParseOptions(LanguageVersion.Preview));

        var compilation =
            CSharpCompilation.Create(
                assemblyName,
                [syntaxTree],
                GetMetadataReferences<object>(),
                new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel:
                    OptimizationLevel.Release,
                    nullableContextOptions:
                    NullableContextOptions.Enable,
                    allowUnsafe: true));

        using var pe = new MemoryStream();

        var emit = compilation.Emit(pe);

        if (!emit.Success)
        {
            throw CreateCompilationException(
                source,
                generatedSource,
                emit.Diagnostics);
        }

        pe.Position = 0;

        var assembly = AssemblyLoadContext.Default.LoadFromStream(pe);

        var type = assembly.GetType("__DynamicEval", throwOnError: true)!;

        var method = type.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Static)!;

        var invoke = method.CreateDelegate<Func<object?>>();

        return invoke();
    }

    private sealed record ParsedMembers(
        IReadOnlyList<MethodDeclarationSyntax> Methods,
        IReadOnlyList<PropertyDeclarationSyntax> Properties);
}
