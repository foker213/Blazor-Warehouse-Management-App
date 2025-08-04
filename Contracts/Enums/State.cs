using System.ComponentModel;

namespace WarehouseManagement.Contracts.Enums;

public enum State
{
    [Description("В работе")]
    InWork,

    [Description("В архиве")]
    InArchive
}
