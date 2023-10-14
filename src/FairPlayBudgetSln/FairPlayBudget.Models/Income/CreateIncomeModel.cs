﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayBudget.Models.Income
{
    public class CreateIncomeModel
    {
        [Required]
        public DateTimeOffset? IncomeDateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string? Description { get; set; }

        [Required]
        public decimal? AmountInUsd { get; set; }
    }
}
