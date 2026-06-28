using SolicitorFinder.Application.Solicitors.SearchSolicitor;
using SolicitorFinder.Models.Requests;

namespace SolicitorFinder.Extensions;

public static class SearchSolicitorMapping
{
    public static SearchSolicitorQuery ToQuery(this SearchSolicitor model)
    {
        return new SearchSolicitorQuery
        {
            LocationId = model.LocationId,
            AreaId = model.AreaId,
            SearchTerm = model.SearchTerm,
            MinRating = model.MinRating,
            MaxRating = model.MaxRating,
            MinReviewCount = model.MinReviewCount,
            MaxReviewCount = model.MaxReviewCount,
            SortBy = model.SortBy,
            SortDescending = model.SortDescending,
            Page = model.Page,
            PageSize = model.PageSize
        };
    }
}
