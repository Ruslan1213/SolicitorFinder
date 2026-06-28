using SolicitorFinder.Services.Models;
namespace SolicitorFinder.Services.Interfaces;

public interface IScraperParserService
{
    Task<List<Solicitor>> ScrapeSolicitorsAsync(SolicitorSearchModel solicitorSearchModel);
}
