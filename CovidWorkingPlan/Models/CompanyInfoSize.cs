using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CovidWorkingPlan.Models
{
    public partial class CompanyInfoSize
    {
        public int IdcompanyInfo { get; set; }
        [Required(ErrorMessage = "Total seatings Nr field is required")]

        public int TotalSeatingNumbers { get; set; }
    }
}
