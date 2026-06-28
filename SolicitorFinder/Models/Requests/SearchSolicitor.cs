using SolicitorFinder.Attributes;

namespace SolicitorFinder.Models.Requests;

public sealed class SearchSolicitor
{
    [ConditionRequired(nameof(AreaId))]
    public int? LocationId { get; set; }

    [ConditionRequired(nameof(LocationId))]
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
