using Arian.Quantiq.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arian.Quantiq.Infrastructure.Persistence.EFCore.Configurations;

public class TableDefinitionConfiguration : IEntityTypeConfiguration<TableDefinition>
{
    public void Configure(EntityTypeBuilder<TableDefinition> builder)
    {
        builder.ToTable("TableDefinitions");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TableName)
            .IsUnique();

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .IsRequired();
    }
}