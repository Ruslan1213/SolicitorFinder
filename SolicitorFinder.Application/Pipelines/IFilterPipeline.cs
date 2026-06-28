using System.Linq.Expressions;

namespace SolicitorFinder.Application.Pipelines;

public interface IFilterPipeline<T>
{
    FilterPipeline<T> AddFilter(IFilter<T> filter);
    FilterPipeline<T> AddFilterIf(bool condition, IFilter<T> filter);
    Expression<Func<T, bool>>? Build();
    void ClearFilters();
}