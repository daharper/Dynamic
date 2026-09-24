using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Dynamic.Runtime;

/// <summary>
/// Parses messages used by the runtime messaging system.
/// </summary>
internal static class RuntimeMessageParser
{
    public static RuntimeMessage Parse(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        source = source.Trim();

        var openParen = source.IndexOf('(');

        if (openParen >= 0)
        {
            return ParseParenthesized(source, openParen);
        }

        return ParseWhitespaceSeparated(source);
    }

    private static RuntimeMessage ParseParenthesized(string source, int openParen)
    {
        if (!source.EndsWith(')'))
        {
            throw new FormatException("Expected ')' at the end of the message.");
        }

        var name = source[..openParen].Trim();

        ValidateName(name);

        var argumentsSource = source[(openParen + 1)..^1];

        var arguments = ParseArguments(argumentsSource, commaSeparated: true);

        return new RuntimeMessage(name, arguments);
    }

    private static RuntimeMessage ParseWhitespaceSeparated(string source)
    {
        var tokens = Tokenize(source, commaSeparated: false);

        if (tokens.Count == 0)
        {
            throw new FormatException("Message cannot be empty.");
        }

        var name = tokens[0];

        ValidateName(name);

        var arguments = tokens
                .Skip(1)
                .Select(ParseValue)
                .ToArray();

        return new RuntimeMessage(name, arguments);
    }

    private static object?[] ParseArguments(string source, bool commaSeparated)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return [];
        }

        return Tokenize(source, commaSeparated)
            .Select(ParseValue)
            .ToArray();
    }

    private static List<string> Tokenize(string source, bool commaSeparated)
    {
        var tokens = new List<string>();

        var current = new StringBuilder();

        char? quote = null;

        for (var index = 0; index < source.Length; index++)
        {
            var character = source[index];

            if (quote is not null)
            {
                current.Append(character);

                if (character == quote)
                {
                    quote = null;
                }

                continue;
            }

            if (character is '"' or '\'')
            {
                quote = character;

                current.Append(character);

                continue;
            }

            var separator = commaSeparated ? character == ',' : char.IsWhiteSpace(character);

            if (!separator)
            {
                current.Append(character);
                continue;
            }

            AddToken(tokens, current);
        }

        if (quote is not null)
        {
            throw new FormatException("Unterminated quoted string.");
        }

        AddToken(tokens, current);

        return tokens;
    }

    private static void AddToken(List<string> tokens, StringBuilder current)
    {
        var token = current.ToString().Trim();

        if (token.Length > 0)
        {
            tokens.Add(token);
        }

        current.Clear();
    }

    private static object? ParseValue(string token)
    {
        if (token.Length >= 2 && token[0] == token[^1] && token[0] is '"' or '\'')
        {
            return token[1..^1];
        }

        if (string.Equals(token, "null", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (bool.TryParse(token, out var boolean))
        {
            return boolean;
        }

        if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            return integer;
        }

        if (decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return decimalValue;
        }

        throw new FormatException($"Could not parse argument '{token}'.");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new FormatException("Message name cannot be empty.");
        }

        if (!SyntaxFacts.IsValidIdentifier(name))
        {
            throw new FormatException($"'{name}' is not a valid message name.");
        }
    }
}
