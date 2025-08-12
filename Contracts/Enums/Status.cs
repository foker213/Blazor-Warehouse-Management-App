using System.ComponentModel;

namespace WarehouseManagement.Contracts.Enums;

public enum Status
{
    [Description("Подписан")]
    Signed,

    [Description("Не подписан")]
    NotSigned
}
