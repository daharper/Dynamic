using System.Dynamic;
using System.Linq.Expressions;

namespace Dynamic.Runtime;

public sealed class ActiveMetaObject : DynamicMetaObject
{
    public ActiveMetaObject(Expression expression, object value)
        : base(expression, BindingRestrictions.GetTypeRestriction(expression, value.GetType()), value)
    {
    }

    public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
    {
        return binder.FallbackInvokeMember(this, args);
    }
}