﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayBudget.DataAccess.Models;

[Index("Description", Name = "UI_MonthlyBudgetInfo_Description", IsUnique = true)]
public partial class MonthlyBudgetInfo
{
    [Key]
    public long MonthlyBudgetInfoId { get; set; }

    [Required]
    [StringLength(150)]
    public string Description { get; set; }

    [InverseProperty("MonthlyBudgetInfo")]
    public virtual ICollection<Expense> Expense { get; set; } = new List<Expense>();

    [InverseProperty("MonthlyBudgetInfo")]
    public virtual ICollection<Income> Income { get; set; } = new List<Income>();
}