using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Configurations;

public sealed class AreaSolicitorConfiguration : IEntityTypeConfiguration<SolicitorArea>
{
    public void Configure(EntityTypeBuilder<SolicitorArea> builder)
    {
        builder
            .HasKey(sa => new { sa.SolicitorId, sa.AreaId });
    }
}
