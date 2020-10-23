using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CovidWorkingPlan.Models
{
    public partial class WorkingToday
    {
        public int IdworkToday { get; set; }
        public int Idemployee { get; set; }
        [Required(ErrorMessage = "Date field is required")]

        [DisplayFormat(DataFormatString = "{0:d}")]

        public DateTime WorkDate { get; set; }

        public int SeatNr { get; set; }

        public virtual EmployeeInfo IdemployeeNavigation { get; set; }
    }
}
