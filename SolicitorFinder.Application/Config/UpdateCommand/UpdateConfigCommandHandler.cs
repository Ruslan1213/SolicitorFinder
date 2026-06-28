using Microsoft.Extensions.Caching.Memory;
using SolicitorFinder.Application.Config.Dtos;
using SolicitorFinder.Application.Location.Dtos;
using SolicitorFinder.Data.Interfaces;
using SolicitorFinder.Data.Models;
using SolicitorFinder.Mediator.Interfaces;

namespace SolicitorFinder.Application.Config.UpdateCommand;

public sealed class UpdateConfigCommandHandler : IRequestHandler<UpdateConfigCommand, ConfigDto>
{
    private readonly IConfigRepository _configRepository;
    private readonly IBaseRepository<Data.Models.Location> _locationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "ApplicationConfig";

    public UpdateConfigCommandHandler(
        IConfigRepository configRepository,
        IBaseRepository<Data.Models.Location> locationRepository,
        IUnitOfWork unitOfWork,
        IMemoryCache cache)
    {
        _configRepository = configRepository;
        _locationRepository = locationRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ConfigDto> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var config = await _configRepository.GetFirstAsync(cancellationToken);

            if (config == null)
            {
                config = new ConfigEntity
                {
                    UpdateInterval = request.UpdateInterval,
                    AutoUpdate = request.AutoUpdate,
                    MaxResults = request.MaxResults,
                    Locations = new List<Data.Models.Location>()
                };
                await _configRepository.AddAsync(config, cancellationToken);
            }
            else
            {
                config.UpdateInterval = request.UpdateInterval;
                config.AutoUpdate = request.AutoUpdate;
                config.MaxResults = request.MaxResults;
            }

            config.Locations.Clear();

            if (request.Locations != null && request.Locations.Any())
            {
                foreach (var locationDto in request.Locations)
                {
                    var existingLocation = await _locationRepository.GetAsync(
                        l => l.Title == locationDto.Title && l.Text == locationDto.Text,
                        cancellationToken
                    );

                    if (existingLocation != null)
                    {
                        existingLocation.ConfigEntityId = config.Id;
                        existingLocation.Config = config;
                        config.Locations.Add(existingLocation);
                    }
                    else
                    {
                        var newLocation = new Data.Models.Location
                        {
                            Title = locationDto.Title,
                            Text = locationDto.Text,
                            ConfigEntityId = config.Id,
                            Config = config
                        };
                        config.Locations.Add(newLocation);
                    }
                }
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _cache.Remove(CacheKey);

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
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
