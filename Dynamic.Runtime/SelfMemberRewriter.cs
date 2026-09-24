using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dynamic.Runtime;

/// <summary>
/// Rewrites unqualified member accesses in dynamically compiled code
/// to explicitly target the current object instance.
/// </summary>
public sealed class SelfMemberRewriter : CSharpSyntaxRewriter
{
    private readonly HashSet<string> _clrMembers;
    private readonly HashSet<string> _runtimeProperties;
    private readonly HashSet<string> _runtimeMethods;
    private readonly Stack<ScopeFrame> _shadowedScopes = new();

    public SelfMemberRewriter(Type clrType, IEnumerable<string> runtimeProperties, IEnumerable<string> runtimeMethods)
    {
        _clrMembers = clrType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public)
                .Select(static member => member.Name)
                .ToHashSet(StringComparer.Ordinal);

        _runtimeProperties = runtimeProperties.ToHashSet(StringComparer.Ordinal);
        _runtimeMethods = runtimeMethods.ToHashSet(StringComparer.Ordinal);
    }

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node) 
    {
        var parameterNames = node.ParameterList
                .Parameters
                .Select(static parameter => parameter.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(parameterNames);

        try
        {
            return base.VisitMethodDeclaration(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitBlock(BlockSyntax node)
    {
        var localNames = node.Statements
                .SelectMany(GetBlockDeclaredNames)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(localNames);

        try
        {
            return base.VisitBlock(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }
     
    public override SyntaxNode? VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
    {
        var parameterNames = node.ParameterList
                .Parameters
                .Select(static parameter => parameter.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(parameterNames);

        try
        {
            return base.VisitLocalFunctionStatement(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
    {
        var parameterNames = new HashSet<string>(StringComparer.Ordinal)
            {
                node.Parameter.Identifier.ValueText
            };

        PushLexicalScope(parameterNames);

        try
        {
            return base.VisitSimpleLambdaExpression(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
    {
        var parameterNames = node.ParameterList
                .Parameters
                .Select(static parameter => parameter.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(parameterNames);

        try
        {
            return base.VisitParenthesizedLambdaExpression(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
    {
        var parameterNames = node.ParameterList?
                                 .Parameters
                                 .Select(static parameter => parameter.Identifier.ValueText)
                                 .Where(static name => !string.IsNullOrEmpty(name))
                                 .ToHashSet(StringComparer.Ordinal) 
                             ?? [with(StringComparer.Ordinal)];

        PushLexicalScope(parameterNames);

        try
        {
            return base.VisitAnonymousMethodExpression(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitQueryExpression(QueryExpressionSyntax node)
    {
        var rewrittenFromExpression = (ExpressionSyntax) Visit(node.FromClause.Expression)!;
        var rewrittenFrom = node.FromClause.WithExpression(rewrittenFromExpression);

        PushQueryVariable(node.FromClause.Identifier.ValueText);

        try
        {
            var rewrittenBody = (QueryBodySyntax)Visit(node.Body)!;
            return node.WithFromClause(rewrittenFrom).WithBody(rewrittenBody);
        }
        finally
        {
            RemoveQueryScopes();
        }
    }

    public override SyntaxNode? VisitQueryBody(QueryBodySyntax node)
    {
        var rewrittenClauses = new List<QueryClauseSyntax>();

        foreach (var clause in node.Clauses)
        {
            switch (clause)
            {
                case FromClauseSyntax fromClause:
                {
                    var rewrittenExpression = (ExpressionSyntax) Visit(fromClause.Expression)!;

                    rewrittenClauses.Add(fromClause.WithExpression(rewrittenExpression));

                    PushQueryVariable(fromClause.Identifier.ValueText);

                    break;
                }

                case LetClauseSyntax letClause:
                {
                    var rewrittenExpression = (ExpressionSyntax)Visit(letClause.Expression)!;

                    rewrittenClauses.Add(letClause.WithExpression(rewrittenExpression));

                    PushQueryVariable(letClause.Identifier.ValueText);

                    break;
                }

                case JoinClauseSyntax joinClause:
                {
                    var rewrittenInExpression = (ExpressionSyntax)Visit(joinClause.InExpression)!;
                    var rewrittenLeftExpression = (ExpressionSyntax)Visit(joinClause.LeftExpression)!;
                    var rewrittenRightExpression = VisitJoinRightExpression(joinClause);

                    var rewrittenJoin = joinClause
                        .WithInExpression(rewrittenInExpression)
                        .WithLeftExpression(rewrittenLeftExpression)
                        .WithRightExpression(rewrittenRightExpression);

                    rewrittenClauses.Add(rewrittenJoin);

                    if (joinClause.Into is not null)
                    {
                        PushQueryVariable(joinClause.Into.Identifier.ValueText);
                    }
                    else
                    {
                        PushQueryVariable(joinClause.Identifier.ValueText);
                    }

                    break;
                }

                default:
                {
                    rewrittenClauses.Add((QueryClauseSyntax)Visit(clause)!);
                    break;
                }
            }
        }

        var rewrittenSelectOrGroup = (SelectOrGroupClauseSyntax)Visit(node.SelectOrGroup)!;

        QueryContinuationSyntax? rewrittenContinuation = null;

        if (node.Continuation is not null)
        {
            var savedQueryScopes = RemoveQueryScopes();

            PushQueryVariable(node.Continuation.Identifier.ValueText);

            try
            {
                var rewrittenContinuationBody = (QueryBodySyntax)Visit(node.Continuation.Body)!;
                rewrittenContinuation = node.Continuation.WithBody(rewrittenContinuationBody);
            }
            finally
            {
                RemoveQueryScopes();
                RestoreQueryScopes(savedQueryScopes);
            }
        }

        return node
            .WithClauses(SyntaxFactory.List(rewrittenClauses))
            .WithSelectOrGroup(rewrittenSelectOrGroup)
            .WithContinuation(rewrittenContinuation);
    }

    public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
    {
        var rewrittenExpression = (ExpressionSyntax)Visit(node.Expression)!;

        var names = new HashSet<string>(StringComparer.Ordinal)
            {
                node.Identifier.ValueText
            };

        PushLexicalScope(names);

        try
        {
            var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;
            return node.WithExpression(rewrittenExpression).WithStatement(rewrittenStatement);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode?
        VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
    {
        var rewrittenExpression = (ExpressionSyntax)Visit(node.Expression)!;

        var variableNames = node.Variable
                .DescendantNodesAndSelf()
                .OfType<SingleVariableDesignationSyntax>()
                .Select(static designation => designation.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(variableNames);

        try
        {
            var rewrittenVariable = (ExpressionSyntax)Visit(node.Variable)!;

            var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;

            return node.WithVariable(rewrittenVariable)
                .WithExpression(rewrittenExpression)
                .WithStatement(rewrittenStatement);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitForStatement(ForStatementSyntax node)
    {
        var loopVariableNames = node.Declaration?
                                    .Variables
                                    .Select(static variable => variable.Identifier.ValueText)
                                    .ToHashSet(StringComparer.Ordinal)
                                ?? [with(StringComparer.Ordinal)];

        var patternVariables = node.Condition?
                                   .DescendantNodesAndSelf()
                                   .OfType<SingleVariableDesignationSyntax>()
                                   .Select(static designation => designation.Identifier.ValueText)
                                   .Where(static name => !string.IsNullOrEmpty(name))
                                   .ToHashSet(StringComparer.Ordinal)
                               ?? [with(StringComparer.Ordinal)];

        var rewrittenDeclaration = node.Declaration is null
                ? null
                : (VariableDeclarationSyntax?)Visit(node.Declaration);

        PushLexicalScope(loopVariableNames);

        try
        {
            var rewrittenInitializers = SyntaxFactory.SeparatedList(
                    node.Initializers.Select(initializer => (ExpressionSyntax)Visit(initializer)!));

            PushLexicalScope(patternVariables);

            try
            {
                var rewrittenCondition = node.Condition is null
                        ? null
                        : (ExpressionSyntax?)Visit(node.Condition);

                var rewrittenIncrementors = SyntaxFactory.SeparatedList(
                        node.Incrementors.Select(incrementor => (ExpressionSyntax)Visit(incrementor)!));

                var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;

                return node
                    .WithDeclaration(rewrittenDeclaration)
                    .WithInitializers(rewrittenInitializers)
                    .WithCondition(rewrittenCondition)
                    .WithIncrementors(rewrittenIncrementors)
                    .WithStatement(rewrittenStatement);
            }
            finally
            {
                _shadowedScopes.Pop();
            }
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitCatchClause(CatchClauseSyntax node)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        if (node.Declaration is not null)
        {
            var name = node.Declaration.Identifier.ValueText;

            if (!string.IsNullOrEmpty(name))
            {
                names.Add(name);
            }
        }

        PushLexicalScope(names);

        try
        {
            return base.VisitCatchClause(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitIfStatement(IfStatementSyntax node)
    {
        var rewrittenCondition = (ExpressionSyntax)Visit(node.Condition)!;

        var patternVariables = GetPatternVariableNames(node.Condition);

        PushLexicalScope(patternVariables);

        try
        {
            var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;

            var rewrittenElse = node.Else is null ? null : (ElseClauseSyntax?)Visit(node.Else);

            return node
                .WithCondition(rewrittenCondition)
                .WithStatement(rewrittenStatement)
                .WithElse(rewrittenElse);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitWhileStatement(WhileStatementSyntax node)
    {
        var patternVariables = GetPatternVariableNames(node.Condition);

        PushLexicalScope(patternVariables);

        try
        {
            var rewrittenCondition = (ExpressionSyntax)Visit(node.Condition)!;
            var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;

            return node.WithCondition(rewrittenCondition).WithStatement(rewrittenStatement);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitDoStatement(DoStatementSyntax node)
    {
        var patternVariables = node.Condition
                .DescendantNodesAndSelf()
                .OfType<SingleVariableDesignationSyntax>()
                .Select(static designation => designation.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;

        PushLexicalScope(patternVariables);

        try
        {
            var rewrittenCondition = (ExpressionSyntax)Visit(node.Condition)!;
            return node.WithStatement(rewrittenStatement).WithCondition(rewrittenCondition);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitSwitchSection(SwitchSectionSyntax node)
    {
        var patternVariables = node.Labels
                .OfType<CasePatternSwitchLabelSyntax>()
                .SelectMany(static label => label.Pattern.DescendantNodesAndSelf().OfType<SingleVariableDesignationSyntax>())
                .Select(static designation => designation.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        if (patternVariables.Count == 0)
        {
            return base.VisitSwitchSection(node);
        }

        PushLexicalScope(patternVariables);

        try
        {
            return base.VisitSwitchSection(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
    {
        var patternVariables = node.Pattern
                .DescendantNodesAndSelf()
                .OfType<SingleVariableDesignationSyntax>()
                .Select(static designation => designation.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        if (patternVariables.Count == 0)
        {
            return base.VisitSwitchExpressionArm(node);
        }

        PushLexicalScope(patternVariables);

        try
        {
            return base.VisitSwitchExpressionArm(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitUsingStatement(UsingStatementSyntax node)
    {
        if (node.Declaration is null)
        {
            return base.VisitUsingStatement(node);
        }

        var variableNames = node.Declaration
                .Variables
                .Select(static variable => variable.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(variableNames);

        try
        {
            var rewrittenDeclaration = (VariableDeclarationSyntax)Visit(node.Declaration)!;

            var rewrittenStatement = (StatementSyntax)Visit(node.Statement)!;

            return node
                .WithDeclaration(rewrittenDeclaration)
                .WithStatement(rewrittenStatement);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        if (!node.IsKind(SyntaxKind.LogicalAndExpression) && !node.IsKind(SyntaxKind.LogicalOrExpression))
        {
            return base.VisitBinaryExpression(node);
        }

        var rewrittenLeft = (ExpressionSyntax)Visit(node.Left)!;

        var patternVariables = node.Left
                .DescendantNodesAndSelf()
                .OfType<SingleVariableDesignationSyntax>()
                .Select(static designation => designation.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        if (patternVariables.Count == 0)
        {
            var rewrittenRight = (ExpressionSyntax)Visit(node.Right)!;

            return node
                .WithLeft(rewrittenLeft)
                .WithRight(rewrittenRight);
        }

        PushLexicalScope(patternVariables);

        try
        {
            var rewrittenRight = (ExpressionSyntax)Visit(node.Right)!;

            return node
                .WithLeft(rewrittenLeft)
                .WithRight(rewrittenRight);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    public override SyntaxNode? VisitConditionalExpression(ConditionalExpressionSyntax node)
    {
        var rewrittenCondition = (ExpressionSyntax)Visit(node.Condition)!;

        var patternVariables = node.Condition
                .DescendantNodesAndSelf()
                .OfType<SingleVariableDesignationSyntax>()
                .Select(static designation => designation.Identifier.ValueText)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);

        if (patternVariables.Count == 0)
        {
            var rewrittenWhenTrue = (ExpressionSyntax)Visit(node.WhenTrue)!;

            var rewrittenWhenFalse = (ExpressionSyntax)Visit(node.WhenFalse)!;

            return node
                .WithCondition(rewrittenCondition)
                .WithWhenTrue(rewrittenWhenTrue)
                .WithWhenFalse(rewrittenWhenFalse);
        }

        var assignedWhenTrue = PatternVariablesAssignedWhenTrue(node.Condition);

        ExpressionSyntax rewrittenTrueBranch;
        ExpressionSyntax rewrittenFalseBranch;

        if (assignedWhenTrue)
        {
            PushLexicalScope(patternVariables);

            try
            {
                rewrittenTrueBranch = (ExpressionSyntax)Visit(node.WhenTrue)!;
            }
            finally
            {
                _shadowedScopes.Pop();
            }

            rewrittenFalseBranch = (ExpressionSyntax)Visit(node.WhenFalse)!;
        }
        else
        {
            rewrittenTrueBranch = (ExpressionSyntax)Visit(node.WhenTrue)!;

            PushLexicalScope(patternVariables);

            try
            {
                rewrittenFalseBranch = (ExpressionSyntax)Visit(node.WhenFalse)!;
            }
            finally
            {
                _shadowedScopes.Pop();
            }
        }

        return node
            .WithCondition(rewrittenCondition)
            .WithWhenTrue(rewrittenTrueBranch)
            .WithWhenFalse(rewrittenFalseBranch);
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var name = node.Identifier.ValueText;


        if (IsShadowed(name))
        {
            return base.VisitIdentifierName(node);
        }

        /*
         * Do not rewrite the name portion of an
         * existing member-access expression.
         *
         * Example:
         *
         *     value.Length
         *
         * Length must not become self.Length.
         */
        if (node.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == node)
        {
            return base.VisitIdentifierName(node);
        }

        /*
         * A real CLR instance member becomes:
         *
         *     self.FirstName
         */
        if (_clrMembers.Contains(name))
        {
            return CreateSelfMemberAccess(node);
        }

        /*
         * A runtime method invocation becomes:
         *
         *     ((dynamic)self).DisplayName()
         */
        if (_runtimeMethods.Contains(name) && node.Parent is InvocationExpressionSyntax invocation && invocation.Expression == node)
        {
            return CreateDynamicSelfMemberAccess(node);
        }

        /*
         * A runtime property becomes:
         *
         *     ((dynamic)self).Nickname
         */
        if (_runtimeProperties.Contains(name))
        {
            return CreateDynamicSelfMemberAccess(node);
        }

        return base.VisitIdentifierName(node);
    }

    private ExpressionSyntax VisitJoinRightExpression(JoinClauseSyntax joinClause)
    {
        var savedQueryScopes = RemoveQueryScopes();

        PushQueryVariable(joinClause.Identifier.ValueText);

        try
        {
            return (ExpressionSyntax)Visit(joinClause.RightExpression)!;
        }
        finally
        {
            RemoveQueryScopes();
            RestoreQueryScopes(savedQueryScopes);
        }
    }

    public override SyntaxNode? VisitFixedStatement(FixedStatementSyntax node)
    {
        var variableNames = node.Declaration
            .Variables
            .Select(static variable => variable.Identifier.ValueText)
            .Where(static name => !string.IsNullOrEmpty(name))
            .ToHashSet(StringComparer.Ordinal);

        PushLexicalScope(variableNames);

        try
        {
            return base.VisitFixedStatement(node);
        }
        finally
        {
            _shadowedScopes.Pop();
        }
    }

    private void PushLexicalScope(IEnumerable<string> names)
    {
        _shadowedScopes.Push(new ScopeFrame(names.ToHashSet(StringComparer.Ordinal), false));
    }

    private void PushQueryVariable(string name)
    {
        if (string.IsNullOrEmpty(name)) return;

        _shadowedScopes.Push(
            new ScopeFrame(
                [
                    with(StringComparer.Ordinal),
                    name
                ],
                true));
    }

    private List<ScopeFrame> RemoveQueryScopes()
    {
        var removed = new List<ScopeFrame>();

        while (_shadowedScopes.Count > 0 && _shadowedScopes.Peek().IsQueryScope)
        {
            removed.Add(_shadowedScopes.Pop());
        }

        return removed;
    }

    private void RestoreQueryScopes(List<ScopeFrame> scopes)
    {
        for (var i = scopes.Count - 1; i >= 0; i--)
        {
            _shadowedScopes.Push(scopes[i]);
        }
    }

    private bool IsShadowed(string name)
    {
        foreach (var scope in _shadowedScopes)
        {
            if (scope.Names.Contains(name))
            {
                return true;
            }
        }

        return false;
    }

    private static HashSet<string> GetPatternVariableNames(ExpressionSyntax expression)
    {
        return expression
            .DescendantNodesAndSelf()
            .OfType<SingleVariableDesignationSyntax>()
            .Select(static designation => designation.Identifier.ValueText)
            .Where(static name => !string.IsNullOrEmpty(name))
            .ToHashSet(StringComparer.Ordinal);
    }

    private static IEnumerable<string> GetBlockDeclaredNames(StatementSyntax statement)
    {
        if (statement is LocalDeclarationStatementSyntax localDeclaration)
        {
            foreach (var variable in localDeclaration.Declaration.Variables)
            {
                yield return variable.Identifier.ValueText;

                /*
                 * Pattern declarations appearing inside
                 * a local initializer remain lexical names
                 * later in the containing block.
                 *
                 * Example:
                 *
                 *     var ok =
                 *         value is not string Nickname ||
                 *         Nickname.Length > 0;
                 */
                if (variable.Initializer is not null)
                {
                    foreach (var designation
                             in variable
                                 .Initializer
                                 .Value
                                 .DescendantNodesAndSelf()
                                 .OfType<SingleVariableDesignationSyntax>())
                    {
                        var name = designation.Identifier.ValueText;

                        if (!string.IsNullOrEmpty(name))
                        {
                            yield return name;
                        }
                    }
                }
            }
        }

        if (statement is LocalFunctionStatementSyntax localFunction)
        {
            yield return localFunction.Identifier.ValueText;
        }

        /*
         * out var declarations and declaration
         * deconstruction.
         *
         * Examples:
         *
         *     out var Nickname
         *
         *     var (Nickname, value) = ...
         */
        foreach (var declarationExpression in statement.DescendantNodes().OfType<DeclarationExpressionSyntax>())
        {
            foreach (var designation in declarationExpression
                         .Designation
                         .DescendantNodesAndSelf()
                         .OfType<SingleVariableDesignationSyntax>())
            {
                var name = designation.Identifier.ValueText;

                if (!string.IsNullOrEmpty(name))
                {
                    yield return name;
                }
            }
        }

        /*
         * Assignment deconstruction.
         *
         * Example:
         *
         *     (Nickname, other) = value;
         */
        if (statement is ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment } &&
            assignment.Left is TupleExpressionSyntax tuple)
        {
            foreach (var identifier in tuple.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
            {
                yield return identifier.Identifier.ValueText;
            }
        }

        /*
         * Pattern variables declared by an if
         * condition remain lexical names after the
         * if statement in the containing block.
         *
         * Example:
         *
         *     if (value is string Nickname)
         *     {
         *     }
         *
         *     return Nickname;
         *
         * The final Nickname must remain bound to the
         * pattern variable. Roslyn then performs the
         * definite-assignment check.
         */
        if (statement is IfStatementSyntax ifStatement)
        {
            foreach (var designation in ifStatement
                         .Condition
                         .DescendantNodesAndSelf()
                         .OfType<SingleVariableDesignationSyntax>())
            {
                var name = designation.Identifier.ValueText;

                if (!string.IsNullOrEmpty(name))
                {
                    yield return name;
                }
            }
        }

        /*
         * Pattern variables declared by a while
         * condition remain lexical names after the
         * while statement in the containing block.
         *
         * Example:
         *
         *     while (value is string Nickname)
         *     {
         *         return Nickname;
         *     }
         *
         *     return Nickname;
         *
         * The final Nickname must remain bound to the
         * pattern variable. Roslyn then performs the
         * definite-assignment check.
         */
        if (statement is WhileStatementSyntax whileStatement)
        {
            foreach (var designation in whileStatement
                         .Condition
                         .DescendantNodesAndSelf()
                         .OfType<SingleVariableDesignationSyntax>())
            {
                var name = designation.Identifier.ValueText;

                if (!string.IsNullOrEmpty(name))
                {
                    yield return name;
                }
            }
        }
    }

    private static bool PatternVariablesAssignedWhenTrue(ExpressionSyntax condition)
    {
        condition = StripParentheses(condition);

        if (condition is PrefixUnaryExpressionSyntax prefix && prefix.IsKind(SyntaxKind.LogicalNotExpression))
        {
            return !PatternVariablesAssignedWhenTrue(prefix.Operand);
        }

        if (condition is IsPatternExpressionSyntax isPattern)
        {
            return !IsNegatedPattern(isPattern.Pattern);
        }

        return true;
    }

    private static ExpressionSyntax
        StripParentheses(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            expression = parenthesized.Expression;
        }

        return expression;
    }

    private static bool IsNegatedPattern(PatternSyntax pattern)
    {
        while (pattern is ParenthesizedPatternSyntax parenthesized)
        {
            pattern = parenthesized.Pattern;
        }

        if (pattern is UnaryPatternSyntax unary && unary.IsKind(SyntaxKind.NotPattern))
        {
            return !IsNegatedPattern(unary.Pattern);
        }

        return false;
    }

    private static MemberAccessExpressionSyntax CreateSelfMemberAccess(IdentifierNameSyntax node)
    {
        return SyntaxFactory
            .MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("self"),
                SyntaxFactory.IdentifierName(node.Identifier))
            .WithTriviaFrom(node);
    }

    private static MemberAccessExpressionSyntax CreateDynamicSelfMemberAccess(IdentifierNameSyntax node)
    {
        var dynamicSelf = SyntaxFactory
                .ParenthesizedExpression(SyntaxFactory.CastExpression(
                    SyntaxFactory.IdentifierName("dynamic"), SyntaxFactory.IdentifierName("self")));

        return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                dynamicSelf,
                SyntaxFactory.IdentifierName(node.Identifier))
            .WithTriviaFrom(node);
    }

    private sealed record ScopeFrame(HashSet<string> Names, bool IsQueryScope);
}