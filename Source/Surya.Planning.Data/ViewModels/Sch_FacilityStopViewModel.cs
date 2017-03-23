using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Surya.India.Model.ViewModel
{
    public class Sch_FacilityStopViewModel
    {
        public int FacilityStopId { get; set; }

        public int FacilityStopHeaderId { get; set; }


        public int FacilityEnrollmentId { get; set; }

        public int AdmissionId { get; set; }

        public DateTime StopDate { get; set; }

        public string FacilityName { get; set; }

        public DateTime StartDate { get; set; }


        public int AvailDays { get; set; }

        [Display(Name = "Stop Reason")]
        public string StopReason { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }


    }
}
