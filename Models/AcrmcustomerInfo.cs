using System;
using System.Collections.Generic;

namespace MobileNumLogin.Models;

public partial class AcrmcustomerInfo
{
    public string RegNo { get; set; } = null!;

    public string? Salutation { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Address { get; set; }

    public string? MobileNo { get; set; }

    public string? PassportNo { get; set; }

    public string? Country { get; set; }

    public string? District { get; set; }

    public string? HomeCity { get; set; }

    public string? HomePhone { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }

    public string? Profession { get; set; }

    public bool? IsWalking { get; set; }

    public bool? IsRegular { get; set; }

    public bool? IsVip { get; set; }

    public string? CustomerType { get; set; }

    public int? BranchId { get; set; }

    public int? UserId { get; set; }

    public int? RefereId { get; set; }

    public int? LoyaltyPointValue { get; set; }

    public string? LoyaltyCardNo { get; set; }

    public DateOnly? LastRedeemDate { get; set; }

    public DateOnly? LoyaltyCardIssueDate { get; set; }

    public bool? IsFilledInitConcern { get; set; }

    public bool? IsDeleted { get; set; }

    public int? IsDeletedBy { get; set; }

    public DateOnly? RegDate { get; set; }

    public DateOnly? SysDate { get; set; }

    public string? StatusRemark { get; set; }

    public string? Remark { get; set; }

    public DateOnly? DoB { get; set; }

    public string? Nic { get; set; }

    public string? SignatureUrl { get; set; }

    public int? EnteredBy { get; set; }

    public DateTime? EnteredDatetime { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDatetime { get; set; }
}
