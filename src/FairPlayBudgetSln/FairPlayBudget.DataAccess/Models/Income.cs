﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FairPlayBudget.DataAccess.Models;

public partial class Income
{
    [Key]
    public long IncomeId { get; set; }

    public DateTimeOffset IncomeDateTime { get; set; }

    [Required]
    [StringLength(50)]
    public string Description { get; set; }

    [Column("AmountInUSD", TypeName = "money")]
    public decimal AmountInUsd { get; set; }

    [Required]
    [StringLength(450)]
    public string OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    [InverseProperty("Income")]
    public virtual AspNetUsers Owner { get; set; }
}