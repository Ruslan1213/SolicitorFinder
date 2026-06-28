using Microsoft.Extensions.Caching.Memory;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using SolicitorFinder.Mediator.Interfaces;
using SolicitorFinder.Services.Interfaces;

namespace SolicitorFinder.Application.Area.SyncArea;

public sealed class SyncAreaCommandHandler : IRequestHandler<SyncAreaCommand, bool>
{
    private readonly IBaseRepository<AreaEntity> _areaRepository;
    private readonly IAreaService _areaService;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "ApplicationAreas";
    private readonly IUnitOfWork _unitOfWork;

    public SyncAreaCommandHandler(
        IBaseRepository<AreaEntity> areaRepository,
        IAreaService areaService,
        IMemoryCache cache,
        IUnitOfWork unitOfWork)
    {
        _areaRepository = areaRepository;
        _areaService = areaService;
        _cache = cache;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SyncAreaCommand request, CancellationToken cancellationToken)
    {
        var areas = await _areaService.GetAreasAsync();

        foreach (var area in areas)
        {
            var existingArea = await _areaRepository.GetAsync(
                a => a.SolicitorAreaExternalId == area.Id,
                cancellationToken);

            if (existingArea == null)
            {
                var newArea = new AreaEntity
                {
                    Name = area.Name,
                    SolicitorAreaExternalId = area.Id
                };

                await _areaRepository.AddAsync(newArea, cancellationToken);
            }
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;

        if (result)
        {
            _cache.Remove(CacheKey);
        }

        return result;
    }
}
