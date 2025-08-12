using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Configurations;

internal sealed class ShipmentDocumentConfiguration : IEntityTypeConfiguration<ShipmentDocument>
{
    public void Configure(EntityTypeBuilder<ShipmentDocument> builder)
    {
        builder.ToTable("ShipmentDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(fw => fw.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Идентификатор");

        builder.Property(x => x.Number)
            .HasMaxLength(40)
            .HasComment("Номер отгрузки");

        builder.Property(x => x.Date)
            .HasComment("Дата отгрузки");

        builder.HasOne(x => x.Client)
            .WithMany(x => x.ShipmentDocuments)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ClientId)
            .HasComment("Прикрепленный клиент");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasComment("Статус");
    }
}
