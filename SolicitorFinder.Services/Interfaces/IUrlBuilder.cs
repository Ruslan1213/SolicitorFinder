using SolicitorFinder.Services.Models;

namespace SolicitorFinder.Services.Interfaces;

public interface IUrlBuilder
{
    string BuildUrl(SolicitorSearchModel solicitorSearchModel);
}

