using Arian.Quantiq.Domain.Entities;
using Arian.Quantiq.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arian.Quantiq.Infrastructure.Persistence.EF.Configurations;

internal class TableDefinitionConfiguration : IEntityTypeConfiguration<TableDefinition>
{
    public void Configure(EntityTypeBuilder<TableDefinition> builder)
    {
        builder.ToTable("TableDefinitions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TableName)
            .IsUnique();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .IsRequired();
    }
}