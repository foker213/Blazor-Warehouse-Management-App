using System.ComponentModel;

namespace WarehouseManagement.Domain.Enums;

public enum Status
{
    [Description("Подписан")]
    Signed,

    [Description("Не подписан")]
    NotSigned
}
