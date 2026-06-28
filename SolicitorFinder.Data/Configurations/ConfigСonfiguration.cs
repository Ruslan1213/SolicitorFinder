using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Configurations;

public sealed class ConfigСonfiguration : IEntityTypeConfiguration<ConfigEntity>
{
    public void Configure(EntityTypeBuilder<ConfigEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(c => c.UpdateInterval);

        builder.Property(c => c.AutoUpdate)
            .HasDefaultValue(true);

        builder.HasMany(c => c.Locations)
            .WithOne(l => l.Config)
            .HasForeignKey(l => l.ConfigEntityId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}