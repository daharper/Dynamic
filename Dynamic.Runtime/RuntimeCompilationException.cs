using Microsoft.CodeAnalysis;

namespace Dynamic.Runtime;

public sealed class RuntimeCompilationException : Exception
{
    public string GeneratedSource { get; }

    public IReadOnlyList<Diagnostic> Diagnostics { get; }

    public RuntimeCompilationException(
        string source,
        string generatedSource,
        IReadOnlyList<Diagnostic> diagnostics)
        : base(CreateMessage(diagnostics))
    {
        Source = source;
        GeneratedSource = generatedSource;
        Diagnostics = diagnostics;
    }

    private static string CreateMessage(IReadOnlyList<Diagnostic> diagnostics)
    {
        return string.Join(Environment.NewLine, diagnostics.Select(static diagnostic => diagnostic.ToString()));
    }
}
