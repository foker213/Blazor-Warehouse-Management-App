using System.ComponentModel;

namespace WarehouseManagement.Domain.Enums;

public enum State
{
    [Description("В работе")]
    InWork,

    [Description("В архиве")]
    InArchive
}
