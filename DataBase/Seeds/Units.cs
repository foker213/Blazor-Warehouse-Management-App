using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Models;

namespace WarehouseManagement.DataBase.Seeds;

internal sealed class Units
{
    public static List<UnitOfMeasure> Get()
    {
        return new List<UnitOfMeasure>
        {
            new UnitOfMeasure
            {
                Name = "КГ",
                State = State.InWork
            },
            new UnitOfMeasure
            {
                Name = "Л",
                State = State.InWork
            },
            new UnitOfMeasure
            {
                Name = "М",
                State = Domain.Enums.State.InWork
            },
            new UnitOfMeasure
            {
                Name = "СМ",
                State = Domain.Enums.State.InArchive
            },
            new UnitOfMeasure
            {
                Name = "Г",
                State = Domain.Enums.State.InArchive
            },
            new UnitOfMeasure
            {
                Name = "МЛ",
                State = Domain.Enums.State.InArchive
            }
        };
    }
}
