namespace Dynamic.Career;

public readonly record struct Money(decimal Amount, string Currency);

public sealed record RightToWork(string Country);

public sealed record Achievement(string Name, string Organisation = "");
