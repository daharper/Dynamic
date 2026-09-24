using System.Reflection;

namespace Dynamic.Runtime;

/// <summary>
/// Provides conversion of arguments used by the runtime invocation system.
/// </summary>
internal static class RuntimeArgumentConverter
{
    public static bool TryPrepareArguments(ParameterInfo[] parameters, object?[] arguments, out RuntimeArgumentBinding binding)
    {
        var hasParamsArray = parameters.Length > 0 && parameters[^1].GetCustomAttribute<ParamArrayAttribute>() is not null;

        if (!hasParamsArray)
        {
            return TryPrepareOrdinaryArguments(parameters, arguments, out binding);
        }

        return TryPrepareParamsArguments(parameters, arguments, out binding);
    }

    private static bool TryPrepareOrdinaryArguments(
        ParameterInfo[] parameters, object?[] arguments,
        out RuntimeArgumentBinding binding)
    {
        if (arguments.Length > parameters.Length)
        {
            binding = default;
            return false;
        }

        for (var i = arguments.Length; i < parameters.Length; i++)
        {
            if (!parameters[i].IsOptional)
            {
                binding = default;
                return false;
            }
        }

        var prepared = new object?[parameters.Length];

        var score = 0;

        for (var i = 0; i < arguments.Length; i++)
        {
            if (!TryPrepareArgument(
                    parameters[i].ParameterType,
                    arguments[i],
                    out var preparedArgument,
                    out var argumentScore))
            {
                binding = default;
                return false;
            }

            prepared[i] = preparedArgument;

            score += argumentScore;
        }

        for (var i = arguments.Length; i < parameters.Length; i++)
        {
            prepared[i] = parameters[i].DefaultValue;
        }

        binding = new RuntimeArgumentBinding(prepared, score);

        return true;
    }

    private static bool TryPrepareParamsArguments(
        ParameterInfo[] parameters,
        object?[] arguments,
        out RuntimeArgumentBinding binding)
    {
        var paramsIndex = parameters.Length - 1;

        var paramsParameter = parameters[paramsIndex];

        var paramsElementType = paramsParameter.ParameterType.GetElementType()!;

        var prepared = new object?[parameters.Length];

        var score = 0;

        var suppliedFixedArgumentCount = Math.Min(arguments.Length, paramsIndex);

        // Prepare the ordinary parameters that were supplied.
        for (var i = 0; i < suppliedFixedArgumentCount; i++)
        {
            if (!TryPrepareArgument(
                    parameters[i].ParameterType,
                    arguments[i],
                    out var preparedArgument,
                    out var argumentScore))
            {
                binding = default;
                return false;
            }

            prepared[i] = preparedArgument;

            score += argumentScore;
        }

        // Any omitted parameters before the params array
        // must be optional.
        for (var i = suppliedFixedArgumentCount; i < paramsIndex; i++)
        {
            if (!parameters[i].IsOptional)
            {
                binding = default;
                return false;
            }

            prepared[i] = parameters[i].DefaultValue;
        }

        /*
         * A params method can be called in normal form:
         *
         *     Sum(new[] { 10, 20, 30 })
         *
         * In that case the final argument is already an int[].
         *
         * Try that before using expanded form.
         */
        if (arguments.Length == parameters.Length)
        {
            var finalArgument = arguments[paramsIndex];

            if (TryPrepareArgument(
                    paramsParameter.ParameterType,
                    finalArgument,
                    out var preparedArgument,
                    out var argumentScore))
            {
                prepared[paramsIndex] = preparedArgument;

                score += argumentScore;

                binding = new RuntimeArgumentBinding(prepared, score);

                return true;
            }
        }

        /*
         * Expanded form:
         *
         *     Sum(10, 20, 30)
         *
         * becomes:
         *
         *     Sum(new[] { 10, 20, 30 })
         */
        var paramsArgumentCount = Math.Max(0, arguments.Length - paramsIndex);

        var paramsArray = Array.CreateInstance(paramsElementType, paramsArgumentCount);

        for (var i = 0; i < paramsArgumentCount; i++)
        {
            var argument = arguments[paramsIndex + i];

            if (!TryPrepareArgument(
                    paramsElementType,
                    argument,
                    out var preparedArgument,
                    out var argumentScore))
            {
                binding = default;
                return false;
            }

            paramsArray.SetValue(preparedArgument, i);

            score += argumentScore;
        }

        prepared[paramsIndex] = paramsArray;

        binding = new RuntimeArgumentBinding(prepared, score);

        return true;
    }

    private static bool TryPrepareArgument(
        Type parameterType,
        object? argument,
        out object? preparedArgument,
        out int score)
    {
        if (argument is null)
        {
            if (parameterType.IsValueType &&
                Nullable.GetUnderlyingType(parameterType) is null)
            {
                preparedArgument = null;
                score = 0;
                return false;
            }

            preparedArgument = null;
            score = 1;
            return true;
        }

        if (parameterType.IsInstanceOfType(argument))
        {
            preparedArgument = argument;
            score = 2;
            return true;
        }

        if (TryConvertNumeric(argument, parameterType, out var converted))
        {
            preparedArgument = converted;
            score = 1;
            return true;
        }

        preparedArgument = null;
        score = 0;
        return false;
    }

    private static bool TryConvertNumeric(object argument, Type targetType, out object? converted)
    {
        var sourceType = argument.GetType();

        var nonNullableTarget = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (!IsImplicitNumericConversion(sourceType, nonNullableTarget))
        {
            converted = null;
            return false;
        }

        converted = Convert.ChangeType(argument, nonNullableTarget);

        return true;
    }

    private static bool IsImplicitNumericConversion(Type sourceType, Type targetType)
    {
        return Type.GetTypeCode(sourceType) switch
        {
            TypeCode.SByte =>
                targetType == typeof(short) ||
                targetType == typeof(int) ||
                targetType == typeof(long) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.Byte =>
                targetType == typeof(short) ||
                targetType == typeof(ushort) ||
                targetType == typeof(int) ||
                targetType == typeof(uint) ||
                targetType == typeof(long) ||
                targetType == typeof(ulong) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.Int16 =>
                targetType == typeof(int) ||
                targetType == typeof(long) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.UInt16 =>
                targetType == typeof(int) ||
                targetType == typeof(uint) ||
                targetType == typeof(long) ||
                targetType == typeof(ulong) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.Int32 =>
                targetType == typeof(long) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.UInt32 =>
                targetType == typeof(long) ||
                targetType == typeof(ulong) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.Int64 =>
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.UInt64 =>
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.Char =>
                targetType == typeof(ushort) ||
                targetType == typeof(int) ||
                targetType == typeof(uint) ||
                targetType == typeof(long) ||
                targetType == typeof(ulong) ||
                targetType == typeof(float) ||
                targetType == typeof(double) ||
                targetType == typeof(decimal),

            TypeCode.Single =>
                targetType == typeof(double),

            _ => false
        };
    }
}