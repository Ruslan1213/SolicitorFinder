using System.Linq.Expressions;

namespace SolicitorFinder.Application.Pipelines;

public interface IFilter<T>
{
    Expression<Func<T, bool>>? Apply(Expression<Func<T, bool>>? currentPredicate);

}
