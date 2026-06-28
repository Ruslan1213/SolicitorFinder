using System.Linq.Expressions;

namespace SolicitorFinder.Application.Pipelines.Filters;
public abstract class Filter<T> : IFilter<T>
{
    private readonly Expression<Func<T, bool>>? _filterExpression;

    protected Filter(Expression<Func<T, bool>>? filterExpression)
    {
        _filterExpression = filterExpression;
    }

    public Expression<Func<T, bool>>? Apply(Expression<Func<T, bool>>? currentPredicate)
    {
        if (_filterExpression == null)
            return currentPredicate;

        if (currentPredicate == null)
            return _filterExpression;

        return CombineExpressions(currentPredicate, _filterExpression);
    }

    private Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        var left = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);
        var right = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

        var body = Expression.AndAlso(left!, right!);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
