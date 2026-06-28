using SolicitorFinder.Services.Models;

namespace SolicitorFinder.Services.Interfaces;

public interface IAreaService
{
    Task<List<AreaDto>> GetAreasAsync();
}