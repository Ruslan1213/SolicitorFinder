using Microsoft.Extensions.Caching.Memory;
using SolicitorFinder.Application.Config.Dtos;
using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Config.GetConfig;

public sealed class GetConfigQueryHandler : IRequestHandler<GetConfigQuery, ConfigDto>
{
    private readonly IConfigRepository _configRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "ApplicationConfig";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetConfigQueryHandler(IConfigRepository configRepository, IMemoryCache cache)
    {
        _configRepository = configRepository;
        _cache = cache;
    }

    public async Task<ConfigDto> Handle(GetConfigQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            var config = await _configRepository.GetFirstAsync(cancellationToken);

            if (config == null)
            {
                return new ConfigDto();
            }

            return new ConfigDto
            {
                UpdateInterval = config.UpdateInterval,
                AutoUpdate = config.AutoUpdate,
                MaxResults = config.MaxResults,
                Locations = config.Locations.Select(l => new LocationDto
                {
                    Title = l.Title,
                    Text = l.Text
                }).ToList()
            };
        }) ?? new ConfigDto();
    }
}
