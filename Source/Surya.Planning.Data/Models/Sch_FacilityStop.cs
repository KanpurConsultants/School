using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_FacilityStop : EntityBase, IHistoryLog
    {
        public Sch_FacilityStop()
        {
        }

        [Key]
        public int FacilityStopId { get; set; }

        [Display(Name = "Facility Stop Header")]
        [ForeignKey("FacilityStopHeader")]
        public int FacilityStopHeaderId { get; set; }
        public virtual Sch_FacilityStopHeader FacilityStopHeader { get; set; }

        
        [ForeignKey("FacilityEnrollment"), Display(Name = "Facility Enrollment No")]
        public int FacilityEnrollmentId { get; set; }
        public virtual Sch_FacilityEnrollment FacilityEnrollment { get; set; }

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
