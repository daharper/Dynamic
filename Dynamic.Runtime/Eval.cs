using System.Linq.Expressions;

namespace Dynamic.Runtime;

/// <summary>
/// Provides runtime evaluation of C# expressions and statements.
/// </summary>
public static class Eval
{
    public static object? Run(string source)
        => RuntimeCompiler.Evaluate(source);

    public static T Run<T>(string source)
        => (T)Run(source)!;

    public static object? Run<TScope>(string source, Expression<Func<TScope>> scope) 
        => RuntimeCompiler.Evaluate(source, scope);
}