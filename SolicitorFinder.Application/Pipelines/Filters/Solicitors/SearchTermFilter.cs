namespace SolicitorFinder.Application.Pipelines.Filters.Solicitors;

public sealed class SearchTermFilter : Filter<Data.Models.Solicitor>
{
    public SearchTermFilter(string? searchTerm)
        : base(string.IsNullOrWhiteSpace(searchTerm) ? null :
            (s => (s.Name != null && s.Name.ToLower().Contains(searchTerm.ToLower())) ||
                  (s.Description != null && s.Description.ToLower().Contains(searchTerm.ToLower())) ||
                  (s.Address != null && s.Address.ToLower().Contains(searchTerm.ToLower()))))
    {
    }
}
