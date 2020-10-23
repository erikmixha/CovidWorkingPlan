using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CovidWorkingPlan.Models
{
    public class WorkDayViewModel
    {
        public int IdworkToday { get; set; }
        public int Idemployee { get; set; }

        [Required(ErrorMessage = "Date field is required")]
        public DateTime WorkDate { get; set; }
        public string EmpNameSurname { get; set; }

        [Required(ErrorMessage = "Code field is required")]
        [DisplayFormat(DataFormatString = "{0:d}")]

        public int EmpCode { get; set; }

        public int SeatNr { get; set; }

        public string ErrorMessage { get; set; }


        public virtual EmployeeInfo IdemployeeNavigation { get; set; }
    }
}
