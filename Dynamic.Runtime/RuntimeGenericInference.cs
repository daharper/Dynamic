using System.Reflection;

namespace Dynamic.Runtime;

public static class RuntimeGenericInference
{
    public static Type[] Infer(MethodInfo method, object?[] arguments)
    {
        var genericParameters = method.GetGenericArguments();
        var parameters = method.GetParameters().Skip(1).ToArray();

        var inferred = new Dictionary<Type, Type>();

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;

            var argument = arguments[i]
                           ?? throw new InvalidOperationException(
                               "Cannot infer a generic type argument from null.");

            var argumentType = argument.GetType();

            // T ← string
            if (parameterType.IsGenericParameter)
            {
                AddInference(inferred, parameterType, argumentType);
                continue;
            }

            // IEnumerable<T> ← string[]
            if (parameterType.IsGenericType)
            {
                var genericDefinition =
                    parameterType.GetGenericTypeDefinition();

                Type? matchingType = null;

                if (argumentType.IsGenericType &&
                    argumentType.GetGenericTypeDefinition() == genericDefinition)
                {
                    matchingType = argumentType;
                }
                else
                {
                    matchingType = argumentType
                        .GetInterfaces()
                        .FirstOrDefault(type =>
                            type.IsGenericType &&
                            type.GetGenericTypeDefinition() == genericDefinition);
                }

                if (matchingType is null)
                {
                    continue;
                }

                var parameterArguments =
                    parameterType.GetGenericArguments();

                var argumentArguments =
                    matchingType.GetGenericArguments();

                for (var j = 0; j < parameterArguments.Length; j++)
                {
                    if (parameterArguments[j].IsGenericParameter)
                    {
                        AddInference(inferred, parameterArguments[j], argumentArguments[j]);
                    }
                }
            }
        }

        return genericParameters
            .Select(parameter => inferred.TryGetValue(parameter, out var type)
                    ? type
                    : throw new InvalidOperationException($"Could not infer generic parameter '{parameter.Name}'."))
            .ToArray();
    }

    private static void AddInference(
        Dictionary<Type, Type> inferred,
        Type genericParameter,
        Type inferredType)
    {
        if (inferred.TryGetValue(
                genericParameter,
                out var existingType))
        {
            if (existingType != inferredType)
            {
                throw new InvalidOperationException(
                    $"Conflicting inferences for generic parameter " +
                    $"'{genericParameter.Name}': " +
                    $"'{existingType}' and '{inferredType}'.");
            }

            return;
        }

        inferred[genericParameter] = inferredType;
    }
}