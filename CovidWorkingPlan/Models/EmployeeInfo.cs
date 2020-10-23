using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CovidWorkingPlan.Models
{
    public partial class EmployeeInfo
    {
        public EmployeeInfo()
        {
            WorkingToday = new HashSet<WorkingToday>();
        }

        public int Idemployee { get; set; }
        public string EmpNameSurname { get; set; }

        [Required(ErrorMessage = "Code field is required")]

        public int EmpCode { get; set; }

        public virtual ICollection<WorkingToday> WorkingToday { get; set; }
    }
}
