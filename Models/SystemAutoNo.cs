using System;
using System.Collections.Generic;

namespace MobileNumLogin.Models;

public partial class SystemAutoNo
{
    public string FormType { get; set; } = null!;

    public int AutoNum { get; set; }

    public DateOnly? LastModifiedDate { get; set; }

    public TimeOnly? LastModifiedTime { get; set; }

    public bool? IsDateUpdated { get; set; }

    public bool? IsMorningUpdated { get; set; }

    public bool? IsEveningUpdated { get; set; }
}
