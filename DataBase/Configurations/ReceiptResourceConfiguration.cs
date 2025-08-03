using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Configurations;

internal sealed class ReceiptResourceConfiguration : IEntityTypeConfiguration<ReceiptResource>
{
    public void Configure(EntityTypeBuilder<ReceiptResource> builder)
    {
        builder.ToTable("ReceiptResources");

        builder.HasKey(x => x.Id);

        builder.Property(fw => fw.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Идентификатор");

        builder.Property(x => x.Quantity)
            .HasComment("Количество");

        builder.HasOne(x => x.Resource)
            .WithOne(x => x.ReceiptResource)
            .HasForeignKey<ReceiptResource>(x => x.ResourceId);

        builder.Property(x => x.ResourceId)
            .HasComment("Прикрепленный ресурс");

        builder.HasOne(x => x.UnitOfMeasure)
            .WithOne(x => x.ReceiptResource)
            .HasForeignKey<ReceiptResource>(x => x.UnitOfMeasureId);

        builder.Property(x => x.UnitOfMeasureId)
            .HasComment("Прикрепленная единица измерения");

        builder.HasOne(b => b.ReceiptDocument)          
               .WithMany(a => a.ReceiptResources)         
               .HasForeignKey(b => b.ReceiptDocumentId) 
               .OnDelete(DeleteBehavior.Cascade);
    }
}