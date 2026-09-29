using System.Linq.Expressions;

namespace Dynamic.Runtime;

public static class SpecificationExtractor
{
    public static Specification Extract<T>(Expression<Func<T, bool>> expression)
        => Extract(expression.Body);

    private static Specification Extract(Expression expression)
    {
        var body = (BinaryExpression)expression;

        if (body.NodeType == ExpressionType.AndAlso)
            return new CompositeSpecification(Extract(body.Left), body.NodeType, Extract(body.Right));

        var member = (MemberExpression)body.Left;
        var value = Expression.Lambda(body.Right).Compile().DynamicInvoke();

        return new Criterion(member.Member, body.NodeType, value);
    }
}