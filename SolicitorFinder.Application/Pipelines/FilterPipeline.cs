using System.Linq.Expressions;

namespace SolicitorFinder.Application.Pipelines;

public sealed class FilterPipeline<T> : IFilterPipeline<T>
{
    private readonly List<IFilter<T>> _filters = new();

    public FilterPipeline<T> AddFilter(IFilter<T> filter)
    {
        _filters.Add(filter);

        return this;
    }

    public FilterPipeline<T> AddFilterIf(bool condition, IFilter<T> filter)
    {
        if (condition)
        {
            _filters.Add(filter);
        }

        return this;
    }

    public Expression<Func<T, bool>>? Build()
    {
        Expression<Func<T, bool>>? currentPredicate = null;

        foreach (var filter in _filters)
        {
            currentPredicate = filter.Apply(currentPredicate);
        }

        return currentPredicate;
    }

    public void ClearFilters()
    {
        _filters.Clear();
    }
}
