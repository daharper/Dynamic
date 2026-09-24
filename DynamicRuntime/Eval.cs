namespace Dynamic.Runtime;

/// <summary>
/// Provides runtime evaluation of C# expressions and statements.
/// </summary>
public static class Eval
{
    public static object? Run(string source)
    {
        return RuntimeCompiler.Evaluate(source);
    }

    public static T Run<T>(string source)
    {
        return (T)Run(source)!;
    }
}