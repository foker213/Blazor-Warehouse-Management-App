using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Configurations;

internal sealed class ShipmentResourceConfiguration : IEntityTypeConfiguration<ShipmentResource>
{
    public void Configure(EntityTypeBuilder<ShipmentResource> builder)
    {
        builder.ToTable("ShipmentResources");

        builder.HasKey(x => x.Id);

        builder.Property(fw => fw.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Идентификатор");

        builder.Property(x => x.Quantity)
            .HasComment("Количество");

        builder.HasOne(x => x.Resource)
            .WithOne(x => x.ShipmentResource)
            .HasForeignKey<ShipmentResource>(x => x.ResourceId);

        builder.Property(x => x.ResourceId)
            .HasComment("Прикрепленный ресурс");

        builder.HasOne(x => x.UnitOfMeasure)
            .WithOne(x => x.ShipmentResource)
            .HasForeignKey<ShipmentResource>(x => x.UnitOfMeasureId);

        builder.Property(x => x.UnitOfMeasureId)
            .HasComment("Прикрепленная единица измерения");

        builder.HasOne(b => b.ShipmentDocument)
               .WithMany(a => a.ShipmentResources)
               .HasForeignKey(b => b.ShipmentDocumentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}