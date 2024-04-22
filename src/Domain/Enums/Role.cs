using System.ComponentModel;

namespace Domain.Enums;

public enum Role
{
    [Description("Administrator")]
    Administrator = 0,
    [Description("Common")]
    Common = 1,
}