using SolicitorFinder.Application.Pipelines;
using SolicitorFinder.Application.Pipelines.Filters.Solicitors;
using SolicitorFinder.Application.Solicitors.DTOs;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Mediator.Interfaces;
using System.Linq.Expressions;

namespace SolicitorFinder.Application.Solicitors.SearchSolicitor;

public sealed class SearchSolicitorsQueryHandler : IRequestHandler<SearchSolicitorQuery, PagedResult<SolicitorDto>>
{
    private static readonly Dictionary<string,
        Func<IQueryable<Data.Models.Solicitor>, bool, IOrderedQueryable<Data.Models.Solicitor>>> SortStrategies =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (q, desc) => desc ? q.OrderByDescending(s => s.Name) : q.OrderBy(s => s.Name),
            ["reviewcount"] = (q, desc) => desc ? q.OrderByDescending(s => s.ReviewCount) : q.OrderBy(s => s.ReviewCount),
            ["stars"] = (q, desc) => desc ? q.OrderByDescending(s => s.RatingStars) : q.OrderBy(s => s.RatingStars),
            ["ratingstars"] = (q, desc) => desc ? q.OrderByDescending(s => s.RatingStars) : q.OrderBy(s => s.RatingStars),
        };

    private readonly IReadRepository<Data.Models.Solicitor> _repository;

    public SearchSolicitorsQueryHandler(
        IReadRepository<Data.Models.Solicitor> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SolicitorDto>> Handle(SearchSolicitorQuery request, CancellationToken cancellationToken)
    {
        var filter = BuildFilter(request);
        var orderBy = BuildOrderBy(request);
        var total = await _repository.CountAsync(filter);
        var items = await _repository.GetFilteredAsync(
            predicate: filter,
            orderBy: orderBy,
            skip: (request.Page - 1) * request.PageSize,
            take: request.PageSize,
            cancellationToken: cancellationToken
        );

        return new PagedResult<SolicitorDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static Expression<Func<Data.Models.Solicitor, bool>>? BuildFilter(SearchSolicitorQuery request)
    {
        return new FilterPipeline<Data.Models.Solicitor>()
            .AddFilter(new LocationFilter(request.LocationId))
            .AddFilter(new AreaIdFilter(request.AreaId))
            .AddFilter(new SearchTermFilter(request.SearchTerm))
            .AddFilter(new MinRatingFilter(request.MinRating))
            .AddFilter(new MaxRatingFilter(request.MaxRating))
            .AddFilter(new MinReviewCountFilter(request.MinReviewCount))
            .AddFilter(new MaxReviewCountFilter(request.MaxReviewCount))
            .Build();
    }

    private static Func<IQueryable<Data.Models.Solicitor>, IOrderedQueryable<Data.Models.Solicitor>> BuildOrderBy(
        SearchSolicitorQuery request)
    {
        var key = request.SortBy ?? "ratingstars";
        var strategy = SortStrategies.GetValueOrDefault(key,
            (q, desc) => desc ? q.OrderByDescending(s => s.RatingStars) : q.OrderBy(s => s.RatingStars));

        return query => strategy(query, request.SortDescending);
    }

    private static SolicitorDto MapToDto(Data.Models.Solicitor solicitor) => new()
    {
        Name = solicitor.Name,
        Phone = solicitor.Phone,
        Address = solicitor.Address,
        Description = solicitor.Description,
        Website = solicitor.Website,
        RatingStars = solicitor.RatingStars,
        ReviewCount = solicitor.ReviewCount,
        ScrapedAt = solicitor.ScrapedAt,
    };
}
