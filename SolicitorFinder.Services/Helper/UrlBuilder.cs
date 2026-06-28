using Microsoft.Extensions.Options;
using SolicitorFinder.Services.Configurations;
using SolicitorFinder.Services.Interfaces;
using SolicitorFinder.Services.Models;

namespace SolicitorFinder.Services.Helper;

internal sealed class UrlBuilder : IUrlBuilder
{
    private readonly ScraperConfiguration _config;

    public UrlBuilder(IOptions<ScraperConfiguration> options)
    {
        _config = options.Value;
    }

    public string BuildUrl(SolicitorSearchModel solicitorSearchModel)
    {
        if (string.IsNullOrEmpty(solicitorSearchModel.Location) &&
            string.IsNullOrEmpty(solicitorSearchModel.AreaId))
        {
            throw new ArgumentException("Location and areaId is requiered");
        }

        if (!string.IsNullOrEmpty(solicitorSearchModel.Location) &&
            string.IsNullOrEmpty(solicitorSearchModel.AreaId))
        {
            return $"{_config.BaseUrl}{string.Format(_config.UrlTemplate, solicitorSearchModel.Location.ToLower())}";
        }

        if (string.IsNullOrEmpty(solicitorSearchModel.Location) && !string.IsNullOrEmpty(solicitorSearchModel.AreaId))
        {
            return $"{_config.BaseUrl}{string.Format(_config.UrlOnlyTypeTemplate, solicitorSearchModel.AreaId)}";
        }

        return $"{_config.BaseUrl}{string.Format(_config.UrlTypeTemplate,
                             solicitorSearchModel.AreaId,
                             Uri.EscapeDataString(solicitorSearchModel.Location!))}";
    }
}
