using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Configurations;

public sealed class LocationSolicitorConfiguration : IEntityTypeConfiguration<SolicitorLocation>
{
    public void Configure(EntityTypeBuilder<SolicitorLocation> builder)
    {
        builder
            .HasKey(sa => new { sa.SolicitorId, sa.LocationId });
    }
}
