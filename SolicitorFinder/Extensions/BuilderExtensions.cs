using Microsoft.Extensions.FileProviders;

namespace SolicitorFinder.GeneralParser.Extensions;

public static class BuilderExtensions
{
    public static void AddSapApplication(this WebApplication app)
    {
        var frontendPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "dist");

        if (!Directory.Exists(frontendPath))
        {
            app.Logger.LogWarning("Frontend dist folder not found at {Path}, static files will not be served", frontendPath);
            return;
        }

        var fileProvider = new PhysicalFileProvider(frontendPath);

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            FileProvider = fileProvider,
            RequestPath = string.Empty,
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider,
            RequestPath = string.Empty,
        });

        app.MapFallbackToFile("index.html", new StaticFileOptions
        {
            FileProvider = fileProvider,
        });
    }
}
