using SolicitorFinder.Application.Solicitors.DTOs;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Solicitors.SearchSolicitor;

public sealed class SearchSolicitorQuery : IRequest<PagedResult<SolicitorDto>>
{
    public int? LocationId { get; set; }
    public int? AreaId { get; set; }
    public string? SearchTerm { get; set; }
    public double? MinRating { get; set; }
    public double? MaxRating { get; set; }
    public int? MinReviewCount { get; set; }
    public int? MaxReviewCount { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; }
}
