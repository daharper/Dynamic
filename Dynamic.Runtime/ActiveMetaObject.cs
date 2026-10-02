using System.Dynamic;
using System.Linq.Expressions;

namespace Dynamic.Runtime;

public sealed class ActiveMetaObject : DynamicMetaObject
{
    private readonly DynamicMetaObject _inner;

    public ActiveMetaObject(Expression expression, object value, DynamicMetaObject inner)
        : base(expression, inner.Restrictions, value)
    {
        _inner = inner;
    }

    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        => _inner.BindGetMember(binder);

    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        => _inner.BindSetMember(binder, value);

    public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        => _inner.BindDeleteMember(binder);

    public override DynamicMetaObject BindConvert(ConvertBinder binder)
        => _inner.BindConvert(binder);

    public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
    {
        if (args.Length == 1)
        {
            var property = LimitType.GetProperty(binder.Name);

            if (property?.CanWrite == true)
            {
                var target = Expression.Convert(Expression, LimitType);
                var value = Expression.Convert(args[0].Expression, property.PropertyType);
                var assign = Expression.Assign(Expression.Property(target, property), value);
                var body = Expression.Block(assign, Expression.Convert(target, typeof(object)));
                var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);

                return new DynamicMetaObject(body, restrictions.Merge(args[0].Restrictions));
            }
        }

        return _inner.BindInvokeMember(binder, args);
    }

    public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
        => _inner.BindCreateInstance(binder, args);

    public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        => _inner.BindInvoke(binder, args);

    public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        => _inner.BindBinaryOperation(binder, arg);

    public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        => _inner.BindUnaryOperation(binder);

    public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        => _inner.BindGetIndex(binder, indexes);

    public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        => _inner.BindSetIndex(binder, indexes, value);

    public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
        => _inner.BindDeleteIndex(binder, indexes);
}