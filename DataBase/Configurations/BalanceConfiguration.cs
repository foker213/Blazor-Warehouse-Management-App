using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Configurations;

internal sealed class BalanceConfiguration : IEntityTypeConfiguration<Balance>
{
    public void Configure(EntityTypeBuilder<Balance> builder)
    {
        builder.ToTable("Balances");

        builder.HasKey(x => x.Id);

        builder.Property(fw => fw.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Идентификатор");

        builder.HasOne(x => x.Resource)
           .WithOne(x => x.Balance)
           .HasForeignKey<Balance>(x => x.ResourceId);

        builder.Property(x => x.ResourceId)
            .HasComment("Прикрепленный ресурс");

        builder.HasOne(x => x.UnitOfMeasure)
            .WithOne(x => x.Balance)
            .HasForeignKey<Balance>(x => x.UnitId);

        builder.Property(x => x.UnitId)
            .HasComment("Прикрепленная единица измерения");

        builder.Property(x => x.Quantity)
            .HasComment("Количество");
    }
}
