using System.Linq.Expressions;
using Dynamic.Runtime;
using Dynamic.Tests.Mocks;

namespace Dynamic.Tests.Fixtures;

public class ExpressionTests
{
    [Fact]
    public void Extract_returns_equality_criterion()
    {
        const string name = "Bob";

        Expression<Func<Person, bool>> expression = person => person.FirstName == name;

        var specification = SpecificationExtractor.Extract(expression);
        var criterion = Assert.IsType<Criterion>(specification);

        Assert.Equal(nameof(Person.FirstName), criterion.Member.Name);
        Assert.Equal(ExpressionType.Equal, criterion.Operation);
        Assert.Equal("Bob", criterion.Value);
    }

    [Fact]
    public void Extract_returns_comparison_criterion()
    {
        const int years = 5;

        Expression<Func<Person, bool>> expression = person => person.Years >= years;

        var specification = SpecificationExtractor.Extract(expression);
        var criterion = Assert.IsType<Criterion>(specification);

        Assert.Equal(nameof(Person.Years), criterion.Member.Name);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, criterion.Operation);
        Assert.Equal(5, criterion.Value);
    }

    [Fact]
    public void Extract_can_handle_conjunction()
    {
        const string name = "Bob";
        const int years = 5;

        Expression<Func<Person, bool>> expression = person => person.FirstName == name && person.Years >= years;
        
        var specification = SpecificationExtractor.Extract(expression);
        var composite = Assert.IsType<CompositeSpecification>(specification);

        Assert.Equal(ExpressionType.AndAlso, composite.Operation);

        var left = Assert.IsType<Criterion>(composite.Left);
        var right = Assert.IsType<Criterion>(composite.Right);

        Assert.Equal(nameof(Person.FirstName), left.Member.Name);
        Assert.Equal("Bob", left.Value);

        Assert.Equal(nameof(Person.Years), right.Member.Name);
        Assert.Equal(5, right.Value);
    }
}
