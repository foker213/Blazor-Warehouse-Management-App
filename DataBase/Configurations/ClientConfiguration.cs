using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);

        builder.Property(fw => fw.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Идентификатор");

        builder.Property(x => x.Name)
            .HasMaxLength(40)
            .HasComment("Наименование");

        builder.Property(x => x.Adress)
            .HasMaxLength(120)
            .HasComment("Адрес");

        builder.Property(x => x.State)
            .HasConversion<string>()
            .HasComment("Состояние");
    }
}