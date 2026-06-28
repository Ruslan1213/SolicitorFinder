using Microsoft.Extensions.Caching.Memory;
using SolicitorFinder.Application.Area.Dto;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Area;

public sealed class GetAreasQueryHandler : IRequestHandler<GetAreasQuery, List<AreaInfo>>
{
    private readonly IBaseRepository<AreaEntity> _areaRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "ApplicationAreas";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public GetAreasQueryHandler(IBaseRepository<AreaEntity> areaRepository, IMemoryCache cache)
    {
        _areaRepository = areaRepository;
        _cache = cache;
    }

    public async Task<List<AreaInfo>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            var result = await _areaRepository.GetAllAsync(cancellationToken);

            return result
                .Select(x => new AreaInfo(x.Id, x.Name))
                .ToList();
        }) ?? new List<AreaInfo>();
    }
}
