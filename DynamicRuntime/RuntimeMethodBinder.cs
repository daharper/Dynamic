using System.Reflection;

namespace Dynamic.Runtime;

/// <summary>
/// Provides runtime binding for dynamically defined methods.
/// </summary>
internal static class RuntimeMethodBinder
{
    public static bool TryBind(IEnumerable<MethodInfo> methods, RuntimeMessage message, out RuntimeMethodBinding binding)
    {
        var candidates = methods
                .Where(method => string.Equals(method.Name, message.Name, StringComparison.Ordinal))
                .Where(static method => !method.ContainsGenericParameters)
                .Select(method =>
                {
                    var matches =
                        RuntimeArgumentConverter.TryPrepareArguments(
                            method.GetParameters(),
                            message.Arguments,
                            out var argumentBinding);

                    return new
                    {
                        Matches = matches,
                        Candidate =
                            new ClrMethodCandidate(
                                method,
                                argumentBinding)
                    };
                })
                .Where(static item => item.Matches)
                .Select(static item => item.Candidate)
                .OrderByDescending(static candidate => candidate.Binding.Score)
                .ToArray();

        if (candidates.Length == 0)
        {
            binding = default;
            return false;
        }

        var candidate = ResolveBestCandidate(candidates, message);

        binding = new RuntimeMethodBinding(candidate.Method, candidate.Binding.Arguments);

        return true;
    }

    private static ClrMethodCandidate ResolveBestCandidate(ClrMethodCandidate[] candidates, RuntimeMessage message)
    {
        var candidate = candidates[0];

        var tiedCandidates = candidates
                .Where(item => item.Binding.Score == candidate.Binding.Score)
                .ToArray();

        if (tiedCandidates.Length == 1)
        {
            return candidate;
        }

        ClrMethodCandidate? best = null;

        for (var i = 0; i < tiedCandidates.Length; i++)
        {
            var current = tiedCandidates[i];

            var betterThanAll = true;

            for (var j = 0; j < tiedCandidates.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }

                if (!IsMoreSpecific(current.Method, tiedCandidates[j].Method, message.Arguments))
                {
                    betterThanAll = false;
                    break;
                }
            }

            if (!betterThanAll)
            {
                continue;
            }

            if (best is not null)
            {
                throw CreateAmbiguousMatchException(message.Name);
            }

            best = current;
        }

        if (best is null)
        {
            throw CreateAmbiguousMatchException(message.Name);
        }

        return best.Value;
    }

    private static bool IsMoreSpecific(MethodInfo candidate, MethodInfo other, object?[] arguments)
    {
        var candidateParameters = candidate.GetParameters();

        var otherParameters = other.GetParameters();

        var candidateIsBetter = false;

        for (var i = 0; i < arguments.Length; i++)
        {
            var candidateType = candidateParameters[i].ParameterType;

            var otherType = otherParameters[i].ParameterType;

            if (candidateType == otherType)
            {
                continue;
            }

            if (otherType.IsAssignableFrom(candidateType))
            {
                candidateIsBetter = true;
                continue;
            }

            /*
             * Candidate is not more specific at this
             * argument position.
             */
            return false;
        }

        return candidateIsBetter;
    }

    private static AmbiguousMatchException CreateAmbiguousMatchException(string methodName)
    {
        return new AmbiguousMatchException(
            $"More than one CLR method named " +
            $"'{methodName}' matches the supplied arguments.");
    }
}